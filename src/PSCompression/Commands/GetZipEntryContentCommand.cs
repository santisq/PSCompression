using System;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;
using PSCompression.Exceptions;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.Get, "ZipEntryContent", DefaultParameterSetName = "Stream")]
[OutputType(typeof(string), ParameterSetName = ["Stream"])]
[OutputType(typeof(byte), ParameterSetName = ["Bytes"])]
[Alias("zipgc")]
public sealed class GetZipEntryContentCommand : PSCmdlet, IDisposable
{
    private readonly ZipArchiveCache _cache = new();

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryFile[] ZipEntry { get; set; } = null!;

    [Parameter(ParameterSetName = "Stream")]
    [ArgumentCompleter(typeof(EncodingCompleter))]
    [EncodingTransformation]
    [ValidateNotNullOrEmpty]
    public Encoding Encoding { get; set; } = new UTF8Encoding();

    [Parameter]
    public SwitchParameter Raw { get; set; }

    [Parameter(ParameterSetName = "Bytes")]
    public SwitchParameter AsByteStream { get; set; }

    [Parameter(ParameterSetName = "Bytes")]
    [ValidateNotNullOrEmpty]
    public int BufferSize { get; set; } = 128_000;

    protected override void ProcessRecord()
    {
        foreach (ZipEntryFile entry in ZipEntry)
        {
            try
            {
                ZipContentReader reader = new(GetOrAdd(entry));
                ReadEntry(entry, reader);
            }
            catch (Exception _) when (_ is PipelineStoppedException or FlowControlException)
            {
                throw;
            }
            catch (Exception exception)
            {
                WriteError(exception.ToOpenError(entry.Source));
            }
        }
    }

    private void ReadEntry(ZipEntryFile entry, ZipContentReader reader)
    {
        if (AsByteStream.IsPresent)
        {
            if (Raw.IsPresent)
            {
                WriteObject(reader.ReadAllBytes(entry));
                return;
            }

            WriteObject(
                reader.StreamBytes(entry, BufferSize),
                enumerateCollection: true);
            return;
        }

        if (Raw.IsPresent)
        {
            WriteObject(reader.ReadToEnd(entry, Encoding));
            return;
        }

        WriteObject(
            reader.StreamLines(entry, Encoding),
            enumerateCollection: true);
    }

    private ZipArchive GetOrAdd(ZipEntryFile entry) => _cache.GetOrAdd(entry);

    public void Dispose()
    {
        _cache?.Dispose();
        GC.SuppressFinalize(this);
    }
}
