using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Get, "ZipContent")]
public sealed class GetZipContentCommand : PSCmdlet
{
    public ZipEntryFile[] InputObject { get; set; }

    protected override void ProcessRecord()
    {
        base.ProcessRecord();
    }
}
