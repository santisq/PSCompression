using System;
using System.IO;
using System.IO.Compression;

namespace PSCompression;

public sealed class ZipEntryFile : ZipEntryBase
{
    public override ZipEntryType Type => ZipEntryType.Archive;

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

    internal override string Move(
        string destination,
        ZipArchive zip)
    {
        zip.ThrowIfNotFound(
            path: RelativePath,
            source: Source,
            entry: out ZipArchiveEntry sourceEntry);

        zip.ThrowIfDuplicate(
            path: destination,
            source: Source,
            normalizedPath: out destination);

        ZipArchiveEntry destinationEntry = zip.CreateEntry(destination);
        using (Stream sourceStream = sourceEntry.Open())
        using (Stream destinationStream = destinationEntry.Open())
        {
            sourceStream.CopyTo(destinationStream);
        }
        sourceEntry.Delete();

        return destination;
    }

    internal override string Rename(
        string newname,
        ZipArchive zip)
    {
        newname.ThrowIfInvalidFileNameChar(nameof(newname));
        return Move(
            destination: RelativePath.Replace(Name, newname),
            zip: zip);
    }
}
