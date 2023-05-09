using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Get, "ZipContent", DefaultParameterSetName = "Raw")]
public sealed class GetZipContentCommand : PSCmdlet
{
    private List<string> _content = new();

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
                using ZipEntryStream stream = entry.OpenRead();
                using StreamReader reader = new(stream);


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
