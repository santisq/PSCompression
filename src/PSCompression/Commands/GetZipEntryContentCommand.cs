using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

[Cmdlet(VerbsCommon.Get, "ZipEntryContent", DefaultParameterSetName = "Stream")]
[OutputType(typeof(ZipEntryContent))]
[Alias("gczip")]
public sealed class GetZipEntryContentCommand : PSCmdlet, IDisposable
{
    private readonly Dictionary<string, ZipContentReader> _cache = new();

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryFile[] InputObject { get; set; } = null!;

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

    public void Dispose()
    {
        foreach (ZipContentReader reader in _cache.Values)
        {
            reader?.Dispose();
        }
    }

    private ZipContentReader GetReader(string entrySource)
    {
        if (!_cache.ContainsKey(entrySource))
        {
            _cache[entrySource] = new ZipContentReader(entrySource);
        }

        return _cache[entrySource];
    }

    protected override void ProcessRecord()
    {
        foreach (ZipEntryFile entry in InputObject)
        {
            try
            {
                ZipContentReader reader = GetReader(entry.EntryRelativePath);

                if (AsBytes.IsPresent)
                {
                    if (Stream.IsPresent)
                    {
                        reader.StreamBytes(entry.EntryRelativePath, BufferSize, this);
                        continue;
                    }

                    WriteObject(new ZipEntryContent(
                        entry, reader.ReadAllBytes(entry.EntryRelativePath)));

                    continue;
                }

                if (Stream.IsPresent)
                {
                    reader.StreamLines(entry.EntryRelativePath, Encoding, this);
                    continue;
                }

                if (Raw.IsPresent)
                {
                    WriteObject(new ZipEntryContent(entry,
                        reader.ReadToEnd(entry.EntryRelativePath, Encoding)));

                    continue;
                }

                WriteObject(new ZipEntryContent(entry,
                    reader.ReadAllLines(entry.EntryRelativePath, Encoding)));
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
}
