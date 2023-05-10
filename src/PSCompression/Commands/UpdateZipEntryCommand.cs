using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsData.Update, "ZipEntry")]
public sealed class UpdateZipEntryCommand : PSCmdlet
{
    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryContent InputObject { get; set; } = null!;

    protected override void ProcessRecord()
    {

    }

}
