using System;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

[Cmdlet(VerbsCommon.Set, "ZipEntryContent", DefaultParameterSetName = "StringValue")]
[OutputType(typeof(ZipEntryFile))]
public sealed class SetZipEntryContentCommand : PSCmdlet, IDisposable
{
    private ZipContentWriter? _zipWriter;

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
    [Parameter(ParameterSetName = "ByteStream")]
    public SwitchParameter Append { get; set; }

    [Parameter(ParameterSetName = "ByteStream")]
    public int BufferSize { get; set; } = 128000;

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing()
    {
        try
        {
            if (ParameterSetName == "ByteStream")
            {
                _zipWriter = new(SourceEntry, Append.IsPresent, BufferSize);
                return;
            }

            _zipWriter = new(SourceEntry, Append.IsPresent, Encoding);
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
            if (_zipWriter is null)
            {
                return;
            }

            if (ParameterSetName == "StringValue")
            {
                _zipWriter.WriteLines(LanguagePrimitives.ConvertTo<string[]>(Value));
                return;
            }

            _zipWriter.WriteBytes(LanguagePrimitives.ConvertTo<byte[]>(Value));
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
        _zipWriter?.Dispose();

        if (PassThru.IsPresent && _zipWriter is not null)
        {
            SourceEntry.Refresh();
            WriteObject(SourceEntry);
        }
    }
}
