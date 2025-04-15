using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;
using PSCompression.Exceptions;

namespace PSCompression;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class CommandToCompressedStringBase : PSCmdlet, IDisposable
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
                Write(_writer, InputObject);
                return;
            }

            WriteLines(_writer, InputObject);
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
        _outstream?.Dispose();

        try
        {
            if (_writer is null || _compressStream is null || _outstream is null)
            {
                return;
            }

            if (AsByteStream.IsPresent)
            {
                WriteObject(_outstream.ToArray(), enumerateCollection: false);
                return;
            }

            WriteObject(Convert.ToBase64String(_outstream.ToArray()));
        }
        catch (Exception _) when (_ is PipelineStoppedException or FlowControlException)
        {
            throw;
        }
        catch (Exception exception)
        {
            WriteError(exception.ToWriteError(_outstream));
        }
    }

    private void WriteLines(StreamWriter writer, string[] lines)
    {
        foreach (string line in lines)
        {
            writer.WriteLine(line);
        }
    }

    private void Write(StreamWriter writer, string[] lines)
    {
        foreach (string line in lines)
        {
            writer.Write(line);
        }
    }

    public void Dispose()
    {
        _writer?.Dispose();
        _compressStream?.Dispose();
        _outstream?.Dispose();
    }
}
