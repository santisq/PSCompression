using System;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Get, "ZipContent", DefaultParameterSetName = "Raw")]
public sealed class GetZipContentCommand : PSCmdlet
{
    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryFile[]? InputObject { get; set; }

    [Parameter(ParameterSetName = "Raw")]
    public SwitchParameter Raw { get; set; }

    [Parameter(ParameterSetName = "Stream")]
    public SwitchParameter Stream { get; set; }

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
                ZipEntryContent entryContent = new(entry);

                if (ParameterSetName == "Raw")
                {
                    if (Raw.IsPresent)
                    {
                        WriteObject(entryContent.ReadAllText(null));
                        return;
                    }

                    WriteObject(entryContent.ReadAllLines(null));
                    return;
                }

                WriteObject(entry.ReadLines(), enumerateCollection: true);
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
