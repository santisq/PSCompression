using System;
using System.IO;
using System.IO.Compression;
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
    private const byte GzipPreamble1 = 0x1f;

    private const byte GzipPreamble2 = 0x8b;

    private const byte GzipPreamble3 = 0x08;

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
    private string ReadGzipFramework(string path)
    {
        int marker = 0;
        int b;
        using FileStream fs = File.OpenRead(path);
        MemoryStream outmem = new();

        while ((b = fs.ReadByte()) != -1)
        {
            if (marker == 0 && (byte)b == GzipPreamble1)
            {
                marker++;
                continue;
            }

            if (marker == 1)
            {
                if ((byte)b == GzipPreamble2)
                {
                    marker++;
                    continue;
                }

                marker = 0;
            }

            if (marker == 2)
            {
                marker = 0;

                if ((byte)b != GzipPreamble3)
                {
                    AppendBytes(path, outmem, fs.Position - 3);
                }
            }
        }

        outmem.Seek(0, SeekOrigin.Begin);
        using StreamReader reader = new(outmem);
        return reader.ReadToEnd();
    }

    private void AppendBytes(string path, MemoryStream outmem, long pos)
    {
        using FileStream substream = File.OpenRead(path);
        substream.Seek(pos, SeekOrigin.Begin);
        using GZipStream gzip = new(substream, CompressionMode.Decompress);
        gzip.CopyTo(outmem);
    }

    private string ReadGzipCore(string path)
    {
        using FileStream fs = File.OpenRead(path);
        using GZipStream gzip = new(fs, CompressionMode.Decompress);
        using StreamReader reader = new(gzip);
        return reader.ReadToEnd();
    }
}
