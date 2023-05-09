using System.Diagnostics;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Set, "ZipContent")]
public sealed class SetZipContentCommand : PSCmdlet
{
    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public string[]? InputObject { get; set; }

    [Parameter(Mandatory = true, Position = 0)]
    public ZipEntryFile? ZipEntry { get; set; }

    protected override void ProcessRecord()
    {
        Debug.Assert(InputObject is not null);
        Debug.Assert(ZipEntry is not null);

        foreach(string line in InputObject)
        {

        }
    }
}
