using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsData.Compress, "ZipArchive")]
[OutputType(typeof(FileInfo))]
[Alias("ziparchive")]
public sealed class CompressZipArchiveCommand : PSCmdlet, IDisposable
{
    private const FileShare s_fsmode = FileShare.ReadWrite | FileShare.Delete;

    private bool _isLiteral;

    private string[] _paths = Array.Empty<string>();

    private ZipArchive? _zip;

    private FileStream? _destination;

    private readonly Queue<DirectoryInfo> _queue = new();

    [Parameter(
        ParameterSetName = "PathWithUpdate",
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true)]
    [Parameter(
        ParameterSetName = "PathWithForce",
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true)]
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
        ParameterSetName = "LiteralPathWithUpdate",
        Mandatory = true,
        ValueFromPipelineByPropertyName = true)]
    [Parameter(
        ParameterSetName = "LiteralPathWithForce",
        Mandatory = true,
        ValueFromPipelineByPropertyName = true)]
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

    [Parameter(
        ParameterSetName = "InputBytesWithUpdate",
        Mandatory = true,
        ValueFromPipeline = true)]
    [Parameter(
        ParameterSetName = "InputBytesWithForce",
        Mandatory = true,
        ValueFromPipeline = true)]
    [Parameter(
        ParameterSetName = "InputBytes",
        Mandatory = true,
        ValueFromPipeline = true)]
    public byte[]? InputBytes { get; set; }

    [Parameter(Mandatory = true, Position = 1)]
    [Alias("DestinationPath")]
    public string Destination { get; set; } = null!;

    [Parameter]
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    [Parameter(ParameterSetName = "InputBytesWithUpdate", Mandatory = true)]
    [Parameter(ParameterSetName = "PathWithUpdate", Mandatory = true)]
    [Parameter(ParameterSetName = "LiteralPathWithUpdate", Mandatory = true)]
    public SwitchParameter Update { get; set; }

    [Parameter(ParameterSetName = "InputBytesWithForce", Mandatory = true)]
    [Parameter(ParameterSetName = "PathWithForce", Mandatory = true)]
    [Parameter(ParameterSetName = "LiteralPathWithForce", Mandatory = true)]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

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
        catch (Exception e) when (e is PipelineStoppedException or FlowControlException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(ExceptionHelpers.StreamOpenError(Destination, e));
        }
    }

    protected override void ProcessRecord()
    {
        foreach (string path in _paths.NormalizePath(_isLiteral, this))
        {
            if (path.IsArchive())
            {
                path.GetParent();
            }

            Traverse(new DirectoryInfo(path));
        }
    }

    private void Traverse(DirectoryInfo item)
    {
        _queue.Clear();
    }

    private void CreateEntry(
        FileInfo file,
        ZipArchive zip,
        string relativepath)
    {
        using Stream stream = zip.CreateEntry(relativepath).Open();
        using FileStream fileStream = file.Open(FileMode.Open, FileAccess.Read, s_fsmode);
        fileStream.CopyTo(stream);
    }

    private FileMode GetFileMode()
    {
        if (Update.IsPresent)
        {
            return FileMode.Append;
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

    public void Dispose()
    {
        _zip?.Dispose();
        _destination?.Dispose();
    }
}
