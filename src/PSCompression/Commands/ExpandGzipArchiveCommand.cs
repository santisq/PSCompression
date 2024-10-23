using System;
using System.IO;
using System.Management.Automation;
using System.Text;
using PSCompression.Extensions;
using PSCompression.Exceptions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "GzipArchive")]
[OutputType(
    typeof(string),
    ParameterSetName = ["Path", "LiteralPath"])]
[OutputType(
    typeof(FileInfo),
    ParameterSetName = ["PathDestination", "LiteralPathDestination"])]
[Alias("gzipfromfile")]
public sealed class ExpandGzipArchiveCommand : PSCmdlet, IDisposable
{
    private bool _isLiteral;

    private FileStream? _destination;

    private string[] _paths = [];

    [Parameter(
        ParameterSetName = "Path",
        Position = 0,
        Mandatory = true,
        ValueFromPipeline = true)]
    [Parameter(
        ParameterSetName = "PathDestination",
        Position = 0,
        Mandatory = true,
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
    [Parameter(
        ParameterSetName = "LiteralPathDestination",
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
        Mandatory = true,
        Position = 1,
        ParameterSetName = "PathDestination")]
    [Parameter(
        Mandatory = true,
        Position = 1,
        ParameterSetName = "LiteralPathDestination")]
    [Alias("DestinationPath")]
    public string Destination { get; set; } = null!;

    [Parameter(ParameterSetName = "PathDestination")]
    [Parameter(ParameterSetName = "LiteralPathDestination")]
    [ArgumentCompleter(typeof(EncodingCompleter))]
    [EncodingTransformation]
    [ValidateNotNullOrEmpty]
    public Encoding Encoding { get; set; } = new UTF8Encoding();

    [Parameter(ParameterSetName = "Path")]
    [Parameter(ParameterSetName = "LiteralPath")]
    public SwitchParameter Raw { get; set; }

    [Parameter(ParameterSetName = "PathDestination")]
    [Parameter(ParameterSetName = "LiteralPathDestination")]
    public SwitchParameter PassThru { get; set; }

    [Parameter(ParameterSetName = "PathDestination")]
    [Parameter(ParameterSetName = "LiteralPathDestination")]
    public SwitchParameter Force { get; set; }

    [Parameter(ParameterSetName = "PathDestination")]
    [Parameter(ParameterSetName = "LiteralPathDestination")]
    public SwitchParameter Update { get; set; }

    protected override void BeginProcessing()
    {
        if (Destination is not null && _destination is null)
        {
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
            catch (Exception exception)
            {
                ThrowTerminatingError(exception.ToStreamOpenError(Destination));
            }
        }
    }

    protected override void ProcessRecord()
    {
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
                if (_destination is not null)
                {
                    GzipReaderOps.CopyTo(
                        path: path,
                        isCoreCLR: PSVersionHelper.IsCoreCLR,
                        destination: _destination);

                    continue;
                }

                GzipReaderOps.GetContent(
                    path: path,
                    isCoreCLR: PSVersionHelper.IsCoreCLR,
                    raw: Raw.IsPresent,
                    encoding: Encoding,
                    cmdlet: this);
            }
            catch (Exception exception)
            {
                WriteError(exception.ToOpenError(path));
            }
        }
    }

    protected override void EndProcessing()
    {
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

    public void Dispose() => _destination?.Dispose();
}
