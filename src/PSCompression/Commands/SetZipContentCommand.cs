using System;
using System.IO;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

[Cmdlet(VerbsCommon.Set, "ZipContent", DefaultParameterSetName = "StringValue")]
public sealed class SetZipContentCommand : PSCmdlet, IDisposable
{
    private ZipEntryStream? _stream;

    private StreamWriter? _writer;

    private byte[]? _buffer;

    private int _index;

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public object[] Value { get; set; } = null!;

    [Parameter(Mandatory = true, Position = 0)]
    public ZipEntryFile SourceEntry { get; set; } = null!;

    [Parameter(ParameterSetName = "StringValue")]
    [ArgumentCompleter(typeof(EncodingCompleter))]
    [EncodingTransformation]
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    [Parameter(ParameterSetName = "ByteStream")]
    public SwitchParameter AsByteStream { get; set; }

    [Parameter(ParameterSetName = "StringValue")]
    public SwitchParameter Append { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    private void WriteLines(string[] lines)
    {
        if (_writer is null)
        {
            return;
        }

        foreach (string line in lines)
        {
            _writer.WriteLine(line);
        }
    }

    private void WriteBytes(byte[] bytes)
    {
        if (_buffer is null || _stream is null)
        {
            return;
        }

        foreach (byte b in bytes)
        {
            if (_index == _buffer.Length)
            {
                _stream.Write(_buffer, 0, _index);
                _index = 0;
            }

            _buffer[_index++] = b;
        }
    }

    protected override void BeginProcessing()
    {
        try
        {
            _stream = SourceEntry.OpenWrite();

            if (AsByteStream.IsPresent || !Append.IsPresent)
            {
                _stream.SetLength(0);
            }

            if (ParameterSetName == "ByteStream")
            {
                _buffer = new byte[128000];
                return;
            }

            _writer = new(_stream, Encoding);

            if (Append.IsPresent)
            {
                _writer.BaseStream.Seek(0, SeekOrigin.End);
            }
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(new ErrorRecord(
                e, "StreamOpen", ErrorCategory.OpenError, SourceEntry));
        }
    }

    protected override void ProcessRecord()
    {
        try
        {
            if (ParameterSetName == "StringValue")
            {
                WriteLines(LanguagePrimitives.ConvertTo<string[]>(Value));
                return;
            }

            WriteBytes(LanguagePrimitives.ConvertTo<byte[]>(Value));
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(new ErrorRecord(
                e, "WriteError", ErrorCategory.WriteError, SourceEntry));
        }
    }

    protected override void EndProcessing()
    {
        try
        {
            if (AsByteStream.IsPresent && _index > 0)
            {
                if (_stream is null || _buffer is null)
                {
                    return;
                }

                _stream.Write(_buffer, 0, _index);
            }

            if (PassThru.IsPresent)
            {
                WriteObject(SourceEntry);
            }
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(new ErrorRecord(
                e, "WriteError", ErrorCategory.WriteError, SourceEntry));
        }
    }

    public void Dispose()
    {
        _writer?.Dispose();
        _stream?.Dispose();
    }
}
