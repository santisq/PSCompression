using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;
using PSCompression.Exceptions;
using PSCompression.Extensions;

namespace PSCompression.Abstractions;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class ToCompressedStringCommandBase : PSCmdlet, IDisposable
{
    private StreamWriter? _writer;

    private Stream? _compressStream;

    private readonly MemoryStream _outstream = new();

    [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
    [AllowEmptyString]
    public string[] InputObject { get; set; } = null!;

    [Parameter]
    [ArgumentCompleter(typeof(EncodingCompleter))]
    [EncodingTransformation]
    [ValidateNotNullOrEmpty]
    public Encoding Encoding { get; set; } = new UTF8Encoding();

    [Parameter]
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    [Parameter]
    [Alias("Raw")]
    public SwitchParameter AsByteStream { get; set; }

    [Parameter]
    public SwitchParameter NoNewLine { get; set; }

    protected abstract Stream CreateCompressionStream(
        Stream outputStream,
        CompressionLevel compressionLevel);

    protected override void ProcessRecord()
    {
        try
        {
            _compressStream ??= CreateCompressionStream(_outstream, CompressionLevel);
            _writer ??= new StreamWriter(_compressStream, Encoding);

            if (NoNewLine.IsPresent)
            {
                _writer.WriteContent(InputObject);
                return;
            }

            _writer.WriteLines(InputObject);
        }
        catch (Exception exception)
        {
            WriteError(exception.ToWriteError(InputObject));
        }
    }

    protected override void EndProcessing()
    {
        _writer?.Dispose();
        _compressStream?.Dispose();
        _outstream.Dispose();

        if (AsByteStream)
        {
            // On purpose, we don't want to enumerate the byte[] for efficiency
            WriteObject(_outstream.ToArray(), enumerateCollection: false);
            return;
        }

        WriteObject(Convert.ToBase64String(_outstream.ToArray()));
    }

    public void Dispose()
    {
        _writer?.Dispose();
        _compressStream?.Dispose();
        _outstream.Dispose();
        GC.SuppressFinalize(this);
    }
}
