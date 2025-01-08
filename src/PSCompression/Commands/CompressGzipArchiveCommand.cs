using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Extensions;
using PSCompression.Exceptions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Compress, "GzipArchive", DefaultParameterSetName = "Path")]
[OutputType(typeof(FileInfo))]
[Alias("gziptofile")]
public sealed class CompressGzipArchive : CommandWithPathBase, IDisposable
{
    private FileStream? _destination;

    private GZipStream? _gzip;

    private FileMode Mode
    {
        get => (Update.IsPresent, Force.IsPresent) switch
        {
            (true, _) => FileMode.Append,
            (_, true) => FileMode.Create,
            _ => FileMode.CreateNew
        };
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
        Destination = ResolvePath(Destination).AddExtensionIfMissing(".gz");

        try
        {
            string parent = Destination.GetParent();

            if (!Directory.Exists(parent))
            {
                Directory.CreateDirectory(parent);
            }

            _destination = File.Open(Destination, Mode);
        }
        catch (Exception exception)
        {
            ThrowTerminatingError(exception.ToStreamOpenError(Destination));
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
            catch (Exception exception)
            {
                WriteError(exception.ToWriteError(InputBytes));
            }

            return;
        }

        _gzip ??= new GZipStream(_destination, CompressionLevel);

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
                using FileStream stream = File.OpenRead(path);
                stream.CopyTo(_gzip);
            }
            catch (Exception exception)
            {
                WriteError(exception.ToWriteError(path));
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

    public void Dispose()
    {
        _gzip?.Dispose();
        _destination?.Dispose();
        GC.SuppressFinalize(this);
    }
}
