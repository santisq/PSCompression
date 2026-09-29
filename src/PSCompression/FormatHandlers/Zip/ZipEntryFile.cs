using System.IO;
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Zip;
using PSCompression.Abstractions.Entries;
using PSCompression.Enum;
using PSCompression.Extensions;

namespace PSCompression.FormatHandlers.Zip;

public sealed class ZipEntryFile : ZipEntryBase
{
    internal override string? FormatDirectoryPath { get; }

    public string CompressionRatio { get; }

    public override EntryType Type { get => EntryType.Archive; }

    public string BaseName { get; }

    public string Extension { get; }

    internal ZipEntryFile(ZipEntry entry, string source)
        : base(entry, source)
    {
        CompressionRatio = Length.CalculateRatio(CompressedLength);
        Name = Path.GetFileName(entry.Name);
        BaseName = Path.GetFileNameWithoutExtension(Name);
        Extension = Path.GetExtension(RelativePath);
        FormatDirectoryPath = $"/{Path.GetDirectoryName(RelativePath)?.NormalizeEntryPath()}";
    }

    internal ZipEntryFile(ZipEntry entry, Stream? stream)
        : base(entry, stream)
    {
        CompressionRatio = Length.CalculateRatio(CompressedLength);
        Name = Path.GetFileName(entry.Name);
        BaseName = Path.GetFileNameWithoutExtension(Name);
        Extension = Path.GetExtension(RelativePath);
        FormatDirectoryPath = $"/{Path.GetDirectoryName(RelativePath)?.NormalizeEntryPath()}";
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
}
