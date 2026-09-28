using System.IO;
using System.Management.Automation;
using PSCompression.Abstractions.Commands;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "RarArchive")]
[OutputType(typeof(FileInfo), typeof(DirectoryInfo))]
[Alias("unrar")]
public sealed class ExpandRarArchiveCommand : PathCommandBase
{
    [Parameter(Position = 1)]
    public string? Destination { get; set; }

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }
}
