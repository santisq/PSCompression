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
[Alias("ziparchive")]
public sealed class CompressZipArchiveCommand : PSCmdlet, IDisposable
{
    private const FileShare s_sharemode = FileShare.ReadWrite | FileShare.Delete;

    private bool _isLiteral;

    private string[] _paths = [];

    private ZipArchive? _zip;

    private FileStream? _destination;

    private WildcardPattern[]? _excludePatterns;

    private readonly Queue<DirectoryInfo> _queue = new();

    [Parameter(
        ParameterSetName = "Path",
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true)]
    [SupportsWildcards]
    public string[] Path
    {
        get => _paths;
        set
        {
            _paths = value;
            _isLiteral = false;
        }
    }

    [Parameter(
        ParameterSetName = "LiteralPath",
        Mandatory = true,
        ValueFromPipelineByPropertyName = true)]
    [Alias("PSPath")]
    public string[] LiteralPath
    {
        get => _paths;
        set
        {
            _paths = value;
            _isLiteral = true;
        }
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
        if (!HasZipExtension(Destination))
        {
            Destination += ".zip";
        }

        try
        {
            Destination = Destination.NormalizePath(isLiteral: true, this);
            string parent = Destination.GetParent();

            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }

            _destination = File.Open(Destination, GetFileMode());
            _zip = new ZipArchive(_destination, GetZipMode());
        }
        catch (Exception exception)
        {
            ThrowTerminatingError(exception.ToStreamOpenError(Destination));
        }

        const WildcardOptions wpoptions = WildcardOptions.Compiled
            | WildcardOptions.CultureInvariant
            | WildcardOptions.IgnoreCase;

        if (Exclude is not null)
        {
            _excludePatterns = Exclude
                .Select(e => new WildcardPattern(e, wpoptions))
                .ToArray();
        }
    }

    protected override void ProcessRecord()
    {
        Dbg.Assert(_zip is not null);

        _queue.Clear();
        foreach (string path in _paths.NormalizePath(_isLiteral, this))
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
            share: s_sharemode);

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

    private FileMode GetFileMode()
    {
        if (Update.IsPresent)
        {
            return FileMode.OpenOrCreate;
        }

        if (Force.IsPresent)
        {
            return FileMode.Create;
        }

        return FileMode.CreateNew;
    }

    private ZipArchiveMode GetZipMode()
    {
        if (!Force.IsPresent && !Update.IsPresent)
        {
            return ZipArchiveMode.Create;
        }

        return ZipArchiveMode.Update;
    }

    private bool HasZipExtension(string path) =>
        System.IO.Path.GetExtension(path)
            .Equals(".zip", StringComparison.InvariantCultureIgnoreCase);

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
    }
}
