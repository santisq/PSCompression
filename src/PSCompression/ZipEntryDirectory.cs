using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression;

public sealed class ZipEntryDirectory : ZipEntryBase
{
    public override ZipEntryType Type => ZipEntryType.Directory;

    internal ZipEntryDirectory(ZipArchiveEntry entry, string source)
        : base(entry, source)
    {

    }

    internal override ZipEntryBase? Move(
        string newname,
        ZipArchive zip,
        bool passthru)
    {
        throw new System.NotImplementedException();
    }

    internal override ZipEntryBase? Rename(
        string newname,
        ZipArchive zip,
        bool passthru)
    {
        throw new System.NotImplementedException();
    }
}
