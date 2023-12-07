using System;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

[Cmdlet(VerbsCommon.Get, "ZipEntryContent", DefaultParameterSetName = "Stream")]
[OutputType(typeof(string), ParameterSetName = new[] { "Stream" })]
[OutputType(typeof(byte), ParameterSetName = new[] { "Bytes" })]
[Alias("gczip")]
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
            catch (Exception e) when (e is PipelineStoppedException or FlowControlException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteError(ExceptionHelpers.ZipOpenError(entry.Source, e));
            }
        }
    }

    private void ReadEntry(ZipEntryFile entry, ZipContentReader reader)
    {
        if (AsByteStream.IsPresent)
        {
            if (Raw.IsPresent)
            {
                WriteObject(reader.ReadAllBytes(entry.RelativePath));
                return;
            }

            reader.StreamBytes(entry.RelativePath, BufferSize, this);
            return;
        }

        if (Raw.IsPresent)
        {
            WriteObject(reader.ReadToEnd(entry.RelativePath, Encoding));
            return;
        }

        reader.StreamLines(entry.RelativePath, Encoding, this);
    }

    private ZipArchive GetOrAdd(ZipEntryFile entry) =>
        _cache.GetOrAdd(entry);

    public void Dispose() => _cache?.Dispose();
}
