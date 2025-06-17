using System.IO;
using System.Management.Automation;
using PSCompression.Abstractions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "TarEntry")]
[OutputType(typeof(FileSystemInfo))]
public sealed class ExpandTarEntryCommand : ExpandEntryCommandBase<TarEntryBase>
{
    protected override FileSystemInfo Extract(TarEntryBase entry) =>
        entry.ExtractTo(Destination!, Force);
}
