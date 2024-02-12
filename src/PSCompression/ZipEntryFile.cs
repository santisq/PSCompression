using System.IO;
using System.IO.Compression;
using PSCompression.Extensions;
using PSCompression.Exceptions;

namespace PSCompression;

public sealed class ZipEntryFile : ZipEntryBase
{
    public override ZipEntryType Type => ZipEntryType.Archive;

    public string BaseName => Path.GetFileNameWithoutExtension(Name);

    public string Extension => Path.GetExtension(RelativePath);

    internal ZipEntryFile(ZipArchiveEntry entry, string source)
        : base(entry, source)
    {

    }

    public ZipArchive OpenRead() => ZipFile.OpenRead(Source);

    public ZipArchive OpenWrite() => ZipFile.Open(Source, ZipArchiveMode.Update);

    internal void Refresh()
    {
        using ZipArchive zip = OpenRead();
        Refresh(zip);
    }

    internal void Refresh(ZipArchive zip)
    {
        ZipArchiveEntry entry = zip.GetEntry(RelativePath);
        Length = entry.Length;
        CompressedLength = entry.CompressedLength;
    }

    internal string Rename(string newname, ZipArchive zip)
    {
        newname.ThrowIfInvalidFileNameChar(nameof(newname));

        return Move(
            path: RelativePath,
            destination: this.ChangeName(newname),
            source: Source,
            zip: zip);
    }
}
