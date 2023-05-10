using System;
using System.IO;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Set, "ZipContent", DefaultParameterSetName = "InputString")]
public sealed class SetZipContentCommand : PSCmdlet, IDisposable
{
    private ZipEntryStream _stream = null!;

    private StreamWriter _writer = null!;

    private byte[] _buffer = null!;

    private int _index;

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public object[] InputObject { get; set; } = null!;

    [Parameter(Mandatory = true, Position = 0)]
    public ZipEntryFile ZipEntry { get; set; } = null!;

    [Parameter(ParameterSetName = "ByteStream")]
    public SwitchParameter AsByteStream { get; set; }

    [Parameter(ParameterSetName = "InputString")]
    public SwitchParameter Append { get; set; }

    private void WriteLines(string[] lines)
    {
        foreach (string line in lines)
        {
            _writer.WriteLine(line);
        }
    }

    private void WriteBytes(byte[] bytes)
    {
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
            _stream = ZipEntry.OpenWrite();

            if (AsByteStream.IsPresent || !Append.IsPresent)
            {
                _stream.SetLength(0);
            }

            if (ParameterSetName == "ByteStream")
            {
                _buffer = new byte[128000];
                return;
            }

            _writer = new(_stream);

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
                e, "StreamOpen", ErrorCategory.OpenError, ZipEntry));
        }
    }

    protected override void ProcessRecord()
    {
        try
        {
            if (ParameterSetName == "ByteStream")
            {
                WriteLines(LanguagePrimitives.ConvertTo<string[]>(InputObject));
                return;
            }

            WriteBytes(LanguagePrimitives.ConvertTo<byte[]>(InputObject));
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(new ErrorRecord(
                e, "WriteError", ErrorCategory.WriteError, InputObject));
        }
    }

    protected override void EndProcessing()
    {
        try
        {
            if (AsByteStream.IsPresent && _index > 0)
            {
                _stream.Write(_buffer, 0, _index);
            }
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(new ErrorRecord(
                e, "WriteError", ErrorCategory.WriteError, InputObject));
        }
    }

    public void Dispose()
    {
        _writer?.Dispose();
        _stream?.Dispose();
    }
}
