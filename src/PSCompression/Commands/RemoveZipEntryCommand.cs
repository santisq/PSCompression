using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Remove, "ZipEntry")]
public sealed class RemoveZipEntryCommand : PSCmdlet, IDisposable
{
    private readonly Dictionary<string, ZipArchive> _cache = new();

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryBase[] InputObject { get; set; } = null!;

    protected override void ProcessRecord()
    {
        foreach (ZipEntryBase entry in InputObject)
        {
            try
            {
                if (!_cache.ContainsKey(entry.Source))
                {
                    _cache[entry.Source] = entry.OpenZip(ZipArchiveMode.Update);
                }

                entry.RemoveEntry(_cache[entry.Source]);
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(
                    e, "RemoveEntry", ErrorCategory.NotSpecified, InputObject));
            }
        }
    }

    public void Dispose()
    {
        foreach(ZipArchive zip in _cache.Values)
        {
            zip?.Dispose();
        }
    }
}
