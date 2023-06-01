using System;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

[Cmdlet(VerbsCommon.Get, "ZipEntryContent", DefaultParameterSetName = "Stream")]
[OutputType(typeof(ZipEntryContent))]
[Alias("gczip")]
public sealed class GetZipEntryContentCommand : PSCmdlet, IDisposable
{
    private readonly ZipArchiveCache _cache = new();

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryFile[] ZipEntry { get; set; } = null!;

    [Parameter(ParameterSetName = "Raw")]
    [Parameter(ParameterSetName = "Stream")]
    [ArgumentCompleter(typeof(EncodingCompleter))]
    [EncodingTransformation]
    [ValidateNotNullOrEmpty]
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    [Parameter(ParameterSetName = "Raw")]
    public SwitchParameter Raw { get; set; }

    [Parameter(ParameterSetName = "Stream")]
    [Parameter(ParameterSetName = "Bytes")]
    public SwitchParameter Stream { get; set; }

    [Parameter(ParameterSetName = "Bytes")]
    public SwitchParameter AsBytes { get; set; }

    [Parameter(ParameterSetName = "Bytes")]
    [ValidateNotNullOrEmpty]
    public int BufferSize { get; set; } = 128000;

    protected override void ProcessRecord()
    {
        foreach (ZipEntryFile entry in ZipEntry)
        {
            try
            {
                ZipContentReader reader = new(GetOrAdd(entry));
                ReadEntry(entry, reader);
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(
                    e, "EntryOpen", ErrorCategory.OpenError, entry));
            }
        }
    }

    private void ReadEntry(ZipEntryFile entry, ZipContentReader reader)
    {
        switch ((bytes: AsBytes.IsPresent, stream: Stream.IsPresent, raw: Raw.IsPresent))
        {
            case { bytes: true, stream: true, raw: _ }:
                reader.StreamBytes(entry.EntryRelativePath, BufferSize, this);
                break;

            case { bytes: true, stream: false, raw: _ }:
                WriteObject(ReadAllBytes(entry, reader));
                break;

            case { bytes: _, stream: true, raw: _ }:
                reader.StreamLines(entry.EntryRelativePath, Encoding, this);
                break;

            case { bytes: _, stream: _, raw: true }:
                WriteObject(ReadToEnd(entry, reader));
                break;

            default:
                WriteObject(ReadAllLines(entry, reader));
                break;
        }
    }

    private ZipArchive GetOrAdd(ZipEntryFile entry) =>
        _cache.GetOrAdd(entry, ZipArchiveMode.Read);

    private ZipEntryContent ReadAllLines(ZipEntryFile entry, ZipContentReader reader) =>
        new(entry, reader.ReadAllLines(entry.EntryRelativePath, Encoding));

    private ZipEntryContent ReadToEnd(ZipEntryFile entry, ZipContentReader reader) =>
        new(entry, reader.ReadToEnd(entry.EntryRelativePath, Encoding));

    private static ZipEntryContent ReadAllBytes(ZipEntryFile entry, ZipContentReader reader) =>
        new(entry, reader.ReadAllBytes(entry.EntryRelativePath));

    public void Dispose() => _cache?.Dispose();
}
