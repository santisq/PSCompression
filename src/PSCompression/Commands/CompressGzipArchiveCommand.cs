using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Extensions;
using static PSCompression.Exceptions.ExceptionHelpers;

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

    [Parameter]
    public SwitchParameter Update { get; set; }

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing()
    {
        if (!HasGZipExtension(Destination))
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
            ThrowTerminatingError(StreamOpenError(Destination, e));
        }
    }

    protected override void ProcessRecord()
    {
        Dbg.Assert(_destination is not null);

        if (InputBytes is not null)
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
                WriteError(ZipWriteError(InputBytes, e));
            }

            return;
        }

        _gzip ??= new GZipStream(_destination, CompressionLevel);

        foreach (string path in _paths.NormalizePath(_isLiteral, this))
        {
            if (!path.IsArchive())
            {
                WriteError(NotArchivePathError(
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
                WriteError(ZipWriteError(path, e));
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

    private bool HasGZipExtension(string path) =>
        System.IO.Path.GetExtension(path)
            .Equals(".gz", StringComparison.InvariantCultureIgnoreCase);

    public void Dispose()
    {
        _gzip?.Dispose();
        _destination?.Dispose();
    }
}
