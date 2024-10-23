using System;
using System.Management.Automation;
using System.Text;
using static PSCompression.Exceptions.ExceptionHelpers;

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
    public Encoding Encoding { get; set; } = new UTF8Encoding();

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
            if (AsByteStream.IsPresent)
            {
                _zipWriter = new ZipContentWriter(
                    entry: SourceEntry,
                    append: Append.IsPresent,
                    bufferSize: BufferSize);
                return;
            }

            _zipWriter = new ZipContentWriter(
                entry: SourceEntry,
                append: Append.IsPresent,
                encoding: Encoding);
        }
        catch (Exception exception)
        {
            ThrowTerminatingError(exception.ToStreamOpenError(SourceEntry));
        }
    }

    protected override void ProcessRecord()
    {
        try
        {
            Dbg.Assert(_zipWriter is not null);

            if (AsByteStream.IsPresent)
            {
                _zipWriter.WriteBytes(LanguagePrimitives.ConvertTo<byte[]>(Value));
                return;
            }

            _zipWriter.WriteLines(
                LanguagePrimitives.ConvertTo<string[]>(Value));
        }
        catch (Exception exception)
        {
            ThrowTerminatingError(exception.ToWriteError(SourceEntry));
        }
    }

    protected override void EndProcessing()
    {
        Dbg.Assert(_zipWriter is not null);

        if (!PassThru.IsPresent)
        {
            return;
        }

        try
        {
            _zipWriter.Dispose();
            SourceEntry.Refresh();
            WriteObject(SourceEntry);
        }
        catch (Exception exception)
        {
            ThrowTerminatingError(exception.ToStreamOpenError(SourceEntry));
        }
    }

    public void Dispose() => _zipWriter?.Dispose();
}
