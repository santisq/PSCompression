using System;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Get, "ZipContent")]
public sealed class GetZipContentCommand : PSCmdlet
{
    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryFile[]? InputObject { get; set; }

    protected override void ProcessRecord()
    {
        if (InputObject is null)
        {
            return;
        }

        foreach (ZipEntryFile entry in InputObject)
        {
            try
            {
                entry.OpenRead();
                WriteObject(new ZipContent(entry));
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
            finally
            {
                entry?.Dispose();
            }
        }
    }
}
