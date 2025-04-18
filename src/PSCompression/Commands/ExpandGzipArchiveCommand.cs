using System;
using System.IO;
using System.Management.Automation;
using System.Text;
using PSCompression.Extensions;
using PSCompression.Exceptions;
using ICSharpCode.SharpZipLib.GZip;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "GzipArchive")]
[OutputType(
    typeof(string),
    ParameterSetName = ["Path", "LiteralPath"])]
[OutputType(
    typeof(FileInfo),
    ParameterSetName = ["PathDestination", "LiteralPathDestination"])]
[Alias("fromgzipfile")]
public sealed class ExpandGzipArchiveCommand : CommandWithPathBase, IDisposable
{
    private FileStream? _destination;

    private FileMode FileMode
    {
        get => (Update.IsPresent, Force.IsPresent) switch
        {
            (true, _) => FileMode.Append,
            (_, true) => FileMode.Create,
            _ => FileMode.CreateNew
        };
    }

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
    public override string[] Path
    {
        get => _paths;
        set => _paths = value;
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
    public override string[] LiteralPath
    {
        get => _paths;
        set => _paths = value;
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
        if (Destination is not null)
        {
            try
            {
                Destination = ResolvePath(Destination);
                string parent = Destination.GetParent();

                if (!Directory.Exists(parent))
                {
                    Directory.CreateDirectory(parent);
                }

                _destination = File.Open(Destination, FileMode);
            }
            catch (Exception exception)
            {
                ThrowTerminatingError(exception.ToStreamOpenError(Destination));
            }
        }
    }

    protected override void ProcessRecord()
    {
        foreach (string path in EnumerateResolvedPaths())
        {
            if (!path.IsArchive())
            {
                WriteError(ExceptionHelper.NotArchivePath(
                    path,
                    IsLiteral ? nameof(LiteralPath) : nameof(Path)));

                continue;
            }

            try
            {
                using FileStream source = File.OpenRead(path);
                using GZipInputStream gz = new(source);

                if (_destination is not null)
                {
                    gz.CopyTo(_destination);
                    continue;
                }

                using StreamReader reader = new(gz, Encoding);

                if (Raw)
                {
                    reader.WriteAllToPipeline(this);
                    continue;
                }

                reader.WriteLinesToPipeline(this);
            }
            catch (Exception _) when (_ is PipelineStoppedException or FlowControlException)
            {
                throw;
            }
            catch (Exception exception)
            {
                WriteError(exception.ToOpenError(path));
            }
        }
    }

    protected override void EndProcessing()
    {
        if (_destination is null)
        {
            return;
        }

        _destination.Dispose();

        if (PassThru.IsPresent)
        {
            WriteObject(new FileInfo(_destination.Name));
        }
    }

    public void Dispose()
    {
        _destination?.Dispose();
        GC.SuppressFinalize(this);
    }
}
