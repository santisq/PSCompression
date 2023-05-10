using System;
using System.IO;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Set, "ZipContent")]
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

    [Parameter]
    public SwitchParameter AsByteStream { get; set; }

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
            _stream.SetLength(0);

            if (!AsByteStream.IsPresent)
            {
                _writer = new(_stream);
                return;
            }

            _buffer = new byte[128000];
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
            if (!AsByteStream.IsPresent)
            {
                WriteLines(Array.ConvertAll(InputObject, Convert.ToString));
                return;
            }

            WriteBytes(Array.ConvertAll(InputObject, Convert.ToByte));
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
            if (_index > 0)
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
