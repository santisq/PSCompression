using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression;

public sealed class ZipEntryDirectory : ZipEntryBase
{
    public ZipEntryType EntryType => ZipEntryType.Directory;

    internal ZipEntryDirectory(ZipArchiveEntry entry, string source) :
        base(entry, source)
    { }

    protected override void Move(
        string newname,
        ZipArchive zip,
        PSCmdlet cmdlet)
    {

        throw new System.NotImplementedException();
    }
}
