using System;
using System.IO;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

[Cmdlet(VerbsData.Expand, "GzipArchive")]
[OutputType(
    typeof(string),
    ParameterSetName = new string[2] { "Path", "LiteralPath" }
)]
[OutputType(
    typeof(FileInfo),
    ParameterSetName = new string[2] { "PathDestination", "LiteralPathDestination" }
)]
[Alias("gzipfromfile")]
public sealed class ExpandGzipArchiveCommand : PSCmdlet
{
    private bool _isLiteral;

    private FileStream? _destination;

    private string[] _paths = Array.Empty<string>();

    [Parameter(
        ParameterSetName = "Path",
        Position = 0,
        Mandatory = true,
        ValueFromPipeline = true
    )]
    [Parameter(
        ParameterSetName = "PathDestination",
        Position = 0,
        Mandatory = true,
        ValueFromPipeline = true
    )]
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
        ValueFromPipelineByPropertyName = true
    )]
    [Parameter(
        ParameterSetName = "LiteralPathDestination",
        Mandatory = true,
        ValueFromPipelineByPropertyName = true
    )]
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

    [Parameter(Mandatory = true, ParameterSetName = "PathDestination")]
    [Parameter(Mandatory = true, ParameterSetName = "LiteralPathDestination")]
    [Alias("DestinationPath")]
    public string Destination { get; set; } = null!;

    [Parameter]
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

    protected override void BeginProcessing()
    {
        if (Destination is not null && _destination is null)
        {
            try
            {
                Destination = Destination.NormalizePath(isLiteral: true, this);

                if (!Force.IsPresent && File.Exists(Destination))
                {
                    ThrowTerminatingError(ExceptionHelpers.FileExistsError(Destination));
                }

                string parent = Destination.GetParent();

                if (!Directory.Exists(parent))
                {
                    Directory.CreateDirectory(parent);
                }

                _destination = File.Open(Destination, FileMode.Append);
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                ThrowTerminatingError(ExceptionHelpers.StreamOpenError(Destination, e));
            }
        }
    }

    protected override void ProcessRecord()
    {
        foreach (string path in _paths.NormalizePath(_isLiteral, this))
        {
            if (!path.IsArchive())
            {
                WriteError(ExceptionHelpers.NotArchivePathError(path));
                continue;
            }

            try
            {
                GzipReaderOps.GetContent(
                    path: path,
                    isCoreCLR: PSVersionHelper.IsCoreCLR,
                    raw: Raw.IsPresent,
                    encoding: Encoding,
                    cmdlet: this);
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteError(ExceptionHelpers.ZipOpenError(path, e));
            }
        }
    }
}
