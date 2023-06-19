using System.IO.Compression;

namespace PSCompression;

public sealed class ZipEntryFile : ZipEntryBase
{
    public ZipEntryType EntryType => ZipEntryType.Archive;

    internal ZipEntryFile(ZipArchiveEntry entry, string source) :
        base(entry, source)
    { }

    public ZipArchive OpenRead() => ZipFile.OpenRead(Source);

    public ZipArchive OpenWrite() => ZipFile.Open(Source, ZipArchiveMode.Update);

    internal void Refresh()
    {
        using ZipArchive zip = OpenRead();
        ZipArchiveEntry entry = zip.GetEntry(EntryRelativePath);
        Length = entry.Length;
        CompressedLength = entry.CompressedLength;
    }
}
