using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Get, "ZipContent")]
public sealed class GetZipContentCommand : PSCmdlet
{
    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryFile[]? InputObject { get; set; }

    protected override void ProcessRecord()
    {
        if(InputObject is null)
        {
            return;
        }

        foreach (ZipEntryFile entry in InputObject)
        {
            WriteObject(entry.ReadToEnd());
        }
    }
}
