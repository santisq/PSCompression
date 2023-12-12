using System;
using System.IO;
using System.IO.Compression;

namespace PSCompression;

public sealed class ZipEntryFile : ZipEntryBase
{
    public override ZipEntryType Type => ZipEntryType.Archive;

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

    internal override ZipEntryBase? Move(
        string destination,
        ZipArchive zip,
        bool passthru)
    {
        if (!zip.TryGetEntry(RelativePath, out ZipArchiveEntry sourceEntry))
        {
            throw EntryNotFoundException.Create(RelativePath, Source);
        }

        destination = destination.NormalizeFileEntryPath();
        if (zip.TryGetEntry(destination, out ZipArchiveEntry _))
        {
            throw DuplicatedEntryException.Create(destination, Source);
        }

        ZipArchiveEntry destinationEntry = zip.CreateEntry(destination);
        using (Stream sourceStream = sourceEntry.Open())
        using (Stream destinationStream = destinationEntry.Open())
        {
            sourceStream.CopyTo(destinationStream);
        }
        sourceEntry.Delete();

        if (!passthru)
        {
            return null;
        }

        return new ZipEntryFile(destinationEntry, Source);
    }

    internal override ZipEntryBase? Rename(
        string newname,
        ZipArchive zip,
        bool passthru)
    {
        if (newname.HasInvalidFileNameChar())
        {
            throw new ArgumentException(
                "Cannot rename the specified target, because it represents a path, " +
                "device name or contains invalid File Name characters.",
                nameof(newname));
        }

        return Move(
            destination: RelativePath.Replace(Name, newname),
            zip,
            passthru);
    }
}
