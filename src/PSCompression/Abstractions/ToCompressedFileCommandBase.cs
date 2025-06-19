using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using PSCompression.Extensions;
using PSCompression.Exceptions;
using System.ComponentModel;
using IOPath = System.IO.Path;

namespace PSCompression.Abstractions;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class ToCompressedFileCommandBase<T> : CommandWithPathBase, IDisposable
    where T : IDisposable
{
    private T? _archive;

    private FileStream? _destination;

    private WildcardPattern[]? _excludePatterns;

    private readonly Queue<DirectoryInfo> _queue = new();

    private readonly HashSet<string> _processed = [];

    private bool _disposed;

    private FileMode FileMode
    {
        get => (Update.IsPresent, Force.IsPresent) switch
        {
            (true, _) => FileMode.OpenOrCreate,
            (_, true) => FileMode.Create,
            _ => FileMode.CreateNew
        };
    }

    protected abstract string FileExtension { get; }

    [Parameter(Mandatory = true, Position = 1)]
    [Alias("DestinationPath")]
    public string Destination { get; set; } = null!;

    [Parameter]
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    [Parameter]
    public virtual SwitchParameter Update { get; set; }

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    [Parameter]
    [SupportsWildcards]
    [ValidateNotNullOrEmpty]
    public string[]? Exclude { get; set; }

    protected abstract T CreateCompressionStream(Stream outputStream);

    protected override void BeginProcessing()
    {
        Destination = ResolvePath(Destination)
            .AddExtensionIfMissing(FileExtension);

        try
        {
            Directory.CreateDirectory(IOPath.GetDirectoryName(Destination));

            _destination = File.Open(Destination, FileMode);
            _archive = CreateCompressionStream(_destination);
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
        Dbg.Assert(_archive is not null);
        _queue.Clear();

        foreach (string path in EnumerateResolvedPaths())
        {
            if (ShouldExclude(path) || ItemIsDestination(path, Destination))
            {
                continue;
            }

            if (Directory.Exists(path))
            {
                Traverse(new DirectoryInfo(path), _archive);
                continue;
            }

            FileInfo file = new(path);
            CreateOrUpdateFileEntry(_archive, file, file.Name);
        }
    }

    private void Traverse(DirectoryInfo dir, T archive)
    {
        _queue.Enqueue(dir);
        IEnumerable<FileSystemInfo> enumerator;
        int length = dir.Parent.FullName.Length + 1;

        while (_queue.Count > 0)
        {
            DirectoryInfo current = _queue.Dequeue();
            CreateDirectoryEntry(archive, current, current.RelativeTo(length));

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
                if (ShouldExclude(item.FullName))
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

                CreateOrUpdateFileEntry(archive, file, file.RelativeTo(length));
            }
        }
    }

    protected abstract void CreateDirectoryEntry(
        T archive,
        DirectoryInfo directory,
        string path);

    protected abstract void CreateOrUpdateFileEntry(
        T archive,
        FileInfo file,
        string path);

    protected static FileStream OpenFileStream(FileInfo file) =>
        file.Open(
            mode: FileMode.Open,
            access: FileAccess.Read,
            share: FileShare.ReadWrite | FileShare.Delete);

    private static bool ItemIsDestination(string source, string destination) =>
        source.Equals(destination, StringComparison.InvariantCultureIgnoreCase);

    private bool ShouldExclude(string path)
    {
        if (!_processed.Add(path))
        {
            return true;
        }

        if (_excludePatterns is null)
        {
            return false;
        }

        foreach (WildcardPattern pattern in _excludePatterns)
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
        _archive?.Dispose();
        _destination?.Dispose();

        if (PassThru.IsPresent && _destination is not null)
        {
            WriteObject(new FileInfo(_destination.Name));
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _archive?.Dispose();
            _destination?.Dispose();
        }

        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
