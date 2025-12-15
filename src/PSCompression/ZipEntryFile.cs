using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;
using PSCompression.Abstractions;
using PSCompression.Exceptions;
using PSCompression.Extensions;

namespace PSCompression;

public sealed class ZipEntryFile : ZipEntryBase
{
    public string CompressionRatio { get; }

    public override EntryType Type { get => EntryType.Archive; }

    public string BaseName { get; }

    public string Extension { get; }

    internal ZipEntryFile(ZipEntry entry, string source)
        : base(entry, source)
    {
        CompressionRatio = GetRatio(Length, CompressedLength);
        Name = Path.GetFileName(entry.Name);
        BaseName = Path.GetFileNameWithoutExtension(Name);
        Extension = Path.GetExtension(RelativePath);
    }

    internal ZipEntryFile(ZipEntry entry, Stream? stream)
        : base(entry, stream)
    {
        CompressionRatio = GetRatio(Length, CompressedLength);
        Name = Path.GetFileName(entry.Name);
        BaseName = Path.GetFileNameWithoutExtension(Name);
        Extension = Path.GetExtension(RelativePath);
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

    internal Stream Open(ZipArchive zip)
    {
        zip.ThrowIfNotFound(
            path: RelativePath,
            source: Source,
            out ZipArchiveEntry? entry);

        return entry.Open();
    }

    internal Stream Open(ICSharpCode.SharpZipLib.Zip.ZipFile zip)
    {
        zip.ThrowIfNotFound(
            path: RelativePath,
            source: Source,
            out ZipEntry? entry);

        return zip.GetInputStream(entry);
    }

    internal void Refresh()
    {
        this.ThrowIfFromStream();
        using ZipArchive zip = OpenRead();
        Refresh(zip);
    }

    internal void Refresh(ZipArchive zip)
    {
        if (zip.TryGetEntry(RelativePath, out ZipArchiveEntry? entry))
        {
            Length = entry.Length;
            CompressedLength = entry.CompressedLength;
        }
    }

    protected override string GetFormatDirectoryPath() =>
        $"/{Path.GetDirectoryName(RelativePath)?.NormalizeEntryPath()}";
}
