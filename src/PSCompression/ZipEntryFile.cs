using System.IO;
using System.IO.Compression;

namespace PSCompression;

public sealed class ZipEntryFile : ZipEntryBase
{
    public string CompressionRatio => GetRatio(Length, CompressedLength);

    public override ZipEntryType Type => ZipEntryType.Archive;

    public string BaseName => Path.GetFileNameWithoutExtension(Name);

    public string Extension => Path.GetExtension(RelativePath);

    internal ZipEntryFile(ZipArchiveEntry entry, string source)
        : base(entry, source)
    {

    }

    private static string GetRatio(long size, long compressedSize)
    {
        float compressedRatio = (float)compressedSize / size;

        if (float.IsNaN(compressedRatio))
        {
            compressedRatio = 0;
        }

        return string.Format("{0:F2}%", 100 - (compressedRatio * 100));
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
}
