using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Extensions;
using PSCompression.Exceptions;
using System.ComponentModel;

namespace PSCompression;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class CommandToCompressedFileBase : CommandWithPathBase, IDisposable
{
    protected abstract string FileExtension { get; }

    private FileStream? _destination;

    private Stream? _compressStream;

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
    public virtual CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    [Parameter]
    public SwitchParameter Update { get; set; }

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    protected abstract Stream CreateCompressionStream(
        Stream destinationStream,
        CompressionLevel compressionLevel);

    protected override void BeginProcessing()
    {
        Destination = ResolvePath(Destination).AddExtensionIfMissing(FileExtension);

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

        _compressStream ??= CreateCompressionStream(_destination, CompressionLevel);

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
                stream.CopyTo(_compressStream);
            }
            catch (Exception exception)
            {
                WriteError(exception.ToWriteError(path));
            }
        }
    }

    protected override void EndProcessing()
    {
        _compressStream?.Dispose();
        _destination?.Dispose();

        if (PassThru.IsPresent && _destination is not null)
        {
            WriteObject(new FileInfo(_destination.Name));
        }
    }

    public void Dispose()
    {
        _compressStream?.Dispose();
        _destination?.Dispose();
        GC.SuppressFinalize(this);
    }
}
