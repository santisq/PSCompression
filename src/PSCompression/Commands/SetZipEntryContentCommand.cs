using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;
using PSCompression.Exceptions;
using PSCompression.Extensions;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.Set, "ZipEntryContent", DefaultParameterSetName = "StringValue")]
[OutputType(typeof(ZipEntryFile))]
[Alias("zipsc")]
public sealed class SetZipEntryContentCommand : PSCmdlet, IDisposable
{
    private ZipArchive? _zip;

    private ZipEntryByteWriter? _byteWriter;

    private StreamWriter? _stringWriter;

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
    public int BufferSize { get; set; } = 128_000;

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing()
    {
        try
        {
            _zip = SourceEntry.OpenWrite();
            Stream stream = SourceEntry.Open(_zip);

            if (AsByteStream)
            {
                _byteWriter = new ZipEntryByteWriter(stream, BufferSize, Append);
                return;
            }

            _stringWriter = new StreamWriter(stream, Encoding);

            if (Append)
            {
                _stringWriter.BaseStream.Seek(0, SeekOrigin.End);
                return;
            }

            _stringWriter.BaseStream.SetLength(0);
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
            if (AsByteStream)
            {
                _byteWriter!.WriteBytes(LanguagePrimitives.ConvertTo<byte[]>(Value));
                return;
            }

            _stringWriter!.WriteLines(LanguagePrimitives.ConvertTo<string[]>(Value));
        }
        catch (Exception exception)
        {
            ThrowTerminatingError(exception.ToWriteError(SourceEntry));
        }
    }

    protected override void EndProcessing()
    {
        if (!PassThru) return;

        try
        {
            Dispose();
            SourceEntry.Refresh();
            WriteObject(SourceEntry);
        }
        catch (Exception exception)
        {
            ThrowTerminatingError(exception.ToStreamOpenError(SourceEntry));
        }
    }

    public void Dispose()
    {
        _byteWriter?.Dispose();
        _stringWriter?.Dispose();
        _zip?.Dispose();
        GC.SuppressFinalize(this);
    }
}
