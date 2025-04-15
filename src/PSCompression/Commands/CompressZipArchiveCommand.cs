using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using PSCompression.Extensions;
using PSCompression.Exceptions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Compress, "ZipArchive")]
[OutputType(typeof(FileInfo))]
[Alias("zipcompress")]
public sealed class CompressZipArchiveCommand : CommandWithPathBase, IDisposable
{
    private ZipArchive? _zip;

    private FileStream? _destination;

    private WildcardPattern[]? _excludePatterns;

    private readonly Queue<DirectoryInfo> _queue = new();

    private ZipArchiveMode ZipArchiveMode
    {
        get => Force.IsPresent || Update.IsPresent
            ? ZipArchiveMode.Update
            : ZipArchiveMode.Create;
    }

    private FileMode FileMode
    {
        get => (Update.IsPresent, Force.IsPresent) switch
        {
            (true, _) => FileMode.OpenOrCreate,
            (_, true) => FileMode.Create,
            _ => FileMode.CreateNew
        };
    }

    [Parameter(Mandatory = true, Position = 1)]
    [Alias("DestinationPath")]
    public string Destination { get; set; } = null!;

    [Parameter]
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    [Parameter]
    public SwitchParameter Update { get; set; }

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    [Parameter]
    [SupportsWildcards]
    [ValidateNotNullOrEmpty]
    public string[]? Exclude { get; set; }

    protected override void BeginProcessing()
    {
        Destination = ResolvePath(Destination).AddExtensionIfMissing(".zip");

        try
        {
            string parent = Destination.GetParent();

            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }

            _destination = File.Open(Destination, FileMode);
            _zip = new ZipArchive(_destination, ZipArchiveMode);
        }
        catch (Exception exception)
        {
            ThrowTerminatingError(exception.ToStreamOpenError(Destination));
        }

        if (Exclude is not null)
        {
            const WildcardOptions options = WildcardOptions.Compiled
                | WildcardOptions.CultureInvariant
                | WildcardOptions.IgnoreCase;

            _excludePatterns = [.. Exclude.Select(pattern => new WildcardPattern(pattern, options))];
        }
    }

    protected override void ProcessRecord()
    {
        Dbg.Assert(_zip is not null);
        _queue.Clear();

        foreach (string path in EnumerateResolvedPaths())
        {
            if (ShouldExclude(_excludePatterns, path))
            {
                continue;
            }

            if (!path.IsArchive())
            {
                Traverse(new DirectoryInfo(path), _zip);
                continue;
            }

            FileInfo file = new(path);
            if (Update.IsPresent && _zip.TryGetEntry(file.Name, out ZipArchiveEntry? entry))
            {
                UpdateEntry(file, entry);
                continue;
            }

            CreateEntry(file, _zip, file.Name);
        }
    }

    private void Traverse(DirectoryInfo dir, ZipArchive zip)
    {
        _queue.Enqueue(dir);
        IEnumerable<FileSystemInfo> enumerator;
        int length = dir.Parent.FullName.Length + 1;

        while (_queue.Count > 0)
        {
            DirectoryInfo current = _queue.Dequeue();

            string relative = current.RelativeTo(length);

            if (!Update.IsPresent || !zip.TryGetEntry(relative, out _))
            {
                zip.CreateEntry(current.RelativeTo(length));
            }

            try
            {
                enumerator = current.EnumerateFileSystemInfos();
            }
            catch (Exception exception)
            {
                WriteError(exception.ToEnumerationError(current));
                continue;
            }

            foreach (FileSystemInfo item in enumerator)
            {
                if (ShouldExclude(_excludePatterns, item.FullName))
                {
                    continue;
                }

                if (item is DirectoryInfo directory)
                {
                    _queue.Enqueue(directory);
                    continue;
                }

                FileInfo file = (FileInfo)item;

                if (ItemIsDestination(file.FullName, Destination))
                {
                    continue;
                }

                relative = file.RelativeTo(length);

                if (Update.IsPresent && zip.TryGetEntry(relative, out ZipArchiveEntry? entry))
                {
                    UpdateEntry(file, entry);
                    continue;
                }

                CreateEntry(file, zip, relative);
            }
        }
    }

    private void CreateEntry(
        FileInfo file,
        ZipArchive zip,
        string relativepath)
    {
        try
        {
            using FileStream fileStream = Open(file);
            using Stream stream = zip
                .CreateEntry(relativepath, CompressionLevel)
                .Open();

            fileStream.CopyTo(stream);
        }
        catch (Exception exception)
        {
            WriteError(exception.ToStreamOpenError(file.FullName));
        }
    }

    private static FileStream Open(FileInfo file) =>
        file.Open(
            mode: FileMode.Open,
            access: FileAccess.Read,
            share: FileShare.ReadWrite | FileShare.Delete);

    private void UpdateEntry(
        FileInfo file,
        ZipArchiveEntry entry)
    {
        try
        {
            using FileStream fileStream = Open(file);
            using Stream stream = entry.Open();
            stream.SetLength(0);
            fileStream.CopyTo(stream);
        }
        catch (Exception exception)
        {
            WriteError(exception.ToStreamOpenError(file.FullName));
        }
    }

    private bool ItemIsDestination(string source, string destination) =>
        source.Equals(destination, StringComparison.InvariantCultureIgnoreCase);

    private static bool ShouldExclude(
        WildcardPattern[]? patterns,
        string path)
    {
        if (patterns is null)
        {
            return false;
        }

        foreach (WildcardPattern pattern in patterns)
        {
            if (pattern.IsMatch(path))
            {
                return true;
            }
        }

        return false;
    }

    protected override void EndProcessing()
    {
        _zip?.Dispose();
        _destination?.Dispose();

        if (PassThru.IsPresent && _destination is not null)
        {
            WriteObject(new FileInfo(_destination.Name));
        }
    }

    public void Dispose()
    {
        _zip?.Dispose();
        _destination?.Dispose();
        GC.SuppressFinalize(this);
    }
}
