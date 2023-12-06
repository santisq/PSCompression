using System;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Remove, "ZipEntry", SupportsShouldProcess = true)]
public sealed class RemoveZipEntryCommand : PSCmdlet, IDisposable
{
    private readonly ZipArchiveCache _cache = new(ZipArchiveMode.Update);

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryBase[] InputObject { get; set; } = null!;

    protected override void ProcessRecord()
    {
        foreach (ZipEntryBase entry in InputObject)
        {
            try
            {
                if (ShouldProcess(entry.ToString(), "Remove"))
                {
                    entry.Remove(_cache.GetOrAdd(entry));
                }
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

    public void Dispose() => _cache?.Dispose();
}
