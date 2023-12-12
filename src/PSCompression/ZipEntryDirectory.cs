using System.IO.Compression;

namespace PSCompression;

public sealed class ZipEntryDirectory : ZipEntryBase
{
    public override ZipEntryType Type => ZipEntryType.Directory;

    internal ZipEntryDirectory(ZipArchiveEntry entry, string source)
        : base(entry, source)
    {

    }

    internal override string Move(
        string newname,
        ZipArchive zip)
    {
        throw new System.NotImplementedException();
    }

    internal override string Rename(
        string newname,
        ZipArchive zip)
    {
        throw new System.NotImplementedException();
    }
}
