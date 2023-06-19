using System.IO.Compression;

namespace PSCompression;

public sealed class ZipEntryDirectory : ZipEntryBase
{
    public ZipEntryType EntryType => ZipEntryType.Directory;

    internal ZipEntryDirectory(ZipArchiveEntry entry, string source) :
        base(entry, source)
    { }
}
