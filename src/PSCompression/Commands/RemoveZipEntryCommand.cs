using System;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Remove, "ZipEntry")]
public sealed class RemoveZipEntryCommand : PSCmdlet
{
    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryBase[] InputObject { get; set; } = null!;

    protected override void ProcessRecord()
    {
        foreach (ZipEntryBase entry in InputObject)
        {
            try
            {
                entry.RemoveEntry();
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
}
