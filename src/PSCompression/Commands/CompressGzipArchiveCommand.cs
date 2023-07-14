using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsData.Compress, "GzipArchive", DefaultParameterSetName = "Path")]
[OutputType(typeof(FileInfo))]
[Alias("gziptofile")]
public sealed class CompressGzipArchive : PSCmdlet, IDisposable
{
    private bool _isLiteral;

    private string[] _paths = Array.Empty<string>();

    private FileStream? _destination;

    private GZipStream? _gzip;

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
        ParameterSetName = "RawBytesWithUpdate",
        Mandatory = true,
        ValueFromPipeline = true)]
    [Parameter(
        ParameterSetName = "RawBytesWithForce",
        Mandatory = true,
        ValueFromPipeline = true)]
    [Parameter(
        ParameterSetName = "RawBytes",
        Mandatory = true,
        ValueFromPipeline = true)]
    public byte[]? InputBytes { get; set; }

    [Parameter(Mandatory = true, Position = 1)]
    [Alias("DestinationPath")]
    public string Destination { get; set; } = null!;

    [Parameter]
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    [Parameter(ParameterSetName = "RawBytesWithUpdate", Mandatory = true)]
    [Parameter(ParameterSetName = "PathWithUpdate", Mandatory = true)]
    [Parameter(ParameterSetName = "LiteralPathWithUpdate", Mandatory = true)]
    public SwitchParameter Update { get; set; }

    [Parameter(ParameterSetName = "RawBytesWithForce", Mandatory = true)]
    [Parameter(ParameterSetName = "PathWithForce", Mandatory = true)]
    [Parameter(ParameterSetName = "LiteralPathWithForce", Mandatory = true)]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing()
    {
        if (System.IO.Path.GetExtension(Destination) != ".gz")
        {
            Destination += ".gz";
        }

        try
        {
            Destination = Destination.NormalizePath(isLiteral: true, this);

            string parent = Destination.GetParent();

            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }

            _destination = File.Open(Destination, GetMode());
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
        if (InputBytes is not null && _destination is not null)
        {
            try
            {
                _destination.Write(InputBytes, 0, InputBytes.Length);
            }
            catch (Exception e) when (e is PipelineStoppedException or FlowControlException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteError(ExceptionHelpers.WriteError(InputBytes, e));
            }

            return;
        }

        _gzip ??= new GZipStream(_destination, CompressionLevel);

        foreach (string path in _paths.NormalizePath(_isLiteral, this))
        {
            if (!path.IsArchive())
            {
                WriteError(ExceptionHelpers.NotArchivePathError(
                    path,
                    _isLiteral ? nameof(LiteralPath) : nameof(Path)));

                continue;
            }

            try
            {
                using FileStream stream = File.OpenRead(path);
                stream.CopyTo(_gzip);
            }
            catch (Exception e) when (e is PipelineStoppedException or FlowControlException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteError(ExceptionHelpers.WriteError(path, e));
            }
        }
    }

    protected override void EndProcessing()
    {
        _gzip?.Dispose();
        _destination?.Dispose();

        if (PassThru.IsPresent && _destination is not null)
        {
            WriteObject(new FileInfo(_destination.Name));
        }
    }

    private FileMode GetMode()
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

    public void Dispose()
    {
        _gzip?.Dispose();
        _destination?.Dispose();
    }
}
