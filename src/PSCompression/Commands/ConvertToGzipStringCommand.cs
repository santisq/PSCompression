using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

[Cmdlet(VerbsData.ConvertTo, "GzipString")]
[OutputType(typeof(byte[]), typeof(string))]
[Alias("gziptostring")]
public sealed class ConvertToGzipStringCommand : PSCmdlet, IDisposable
{
    private StreamWriter? _writer;

    private GZipStream? _gzip;

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

    protected override void ProcessRecord()
    {
        try
        {
            _gzip ??= new(_outstream, CompressionLevel);
            _writer ??= new(_gzip, Encoding);

            if (NoNewLine.IsPresent)
            {
                Write(_writer, InputObject);
                return;
            }

            WriteLines(_writer, InputObject);
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            WriteError(new ErrorRecord(
                e, "WriteError", ErrorCategory.NotSpecified, InputObject));
        }
    }

    protected override void EndProcessing()
    {
        _writer?.Dispose();
        _gzip?.Dispose();
        _outstream?.Dispose();

        try
        {
            if (_writer is null || _gzip is null || _outstream is null)
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
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            WriteError(new ErrorRecord(
                e, "OutputError", ErrorCategory.NotSpecified, _outstream));
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
        _gzip?.Dispose();
        _outstream?.Dispose();
    }
}
