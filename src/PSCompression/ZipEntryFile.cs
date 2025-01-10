using System.IO;
using System.IO.Compression;
using PSCompression.Exceptions;
using PSCompression.Extensions;

namespace PSCompression;

public sealed class ZipEntryFile : ZipEntryBase
{
    internal override string FormatDirectoryPath
    {
        get => _formatDirectoryPath ??=
            $"/{Path.GetDirectoryName(RelativePath).NormalizeEntryPath()}";
    }

    public string CompressionRatio => GetRatio(Length, CompressedLength);

    public override ZipEntryType Type => ZipEntryType.Archive;

    public string BaseName => Path.GetFileNameWithoutExtension(Name);

    public string Extension => Path.GetExtension(RelativePath);

    internal ZipEntryFile(ZipArchiveEntry entry, string source)
        : base(entry, source)
    { }

    internal ZipEntryFile(ZipArchiveEntry entry, Stream? stream)
        : base(entry, stream)
    { }

    private static string GetRatio(long size, long compressedSize)
    {
        float compressedRatio = (float)compressedSize / size;

        if (float.IsNaN(compressedRatio))
        {
            compressedRatio = 0;
        }

        return string.Format("{0:F2}%", 100 - (compressedRatio * 100));
    }

    internal Stream Open(ZipArchive zip)
    {
        zip.ThrowIfNotFound(
            path: RelativePath,
            source: Source,
            out ZipArchiveEntry entry);

        return entry.Open();
    }

    internal void Refresh()
    {
        this.ThrowIfFromStream();
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
