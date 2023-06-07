using System;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Remove, "ZipEntry")]
public sealed class RemoveZipEntryCommand : PSCmdlet, IDisposable
{
    private readonly ZipArchiveCache _cache = new();

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryBase[] InputObject { get; set; } = null!;

    protected override void ProcessRecord()
    {
        foreach (ZipEntryBase entry in InputObject)
        {
            try
            {
                entry.RemoveEntry(_cache.GetOrAdd(entry, ZipArchiveMode.Update));
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteError(ExceptionHelpers.ZipOpenError(entry.Source, e));
            }
        }
    }

    public void Dispose() => _cache?.Dispose();
}
