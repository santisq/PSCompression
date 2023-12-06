using System.IO;
using System.IO.Compression;
using System.Management.Automation;

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

    protected override void Move(
        string destination,
        ZipArchive zip,
        PSCmdlet cmdlet)
    {
        if (!zip.TryGetEntry(EntryRelativePath, out ZipArchiveEntry sourceEntry))
        {
            return;
        }

        destination = destination.NormalizeFileEntryPath();
        if (zip.TryGetEntry(destination, out ZipArchiveEntry destinationEntry))
        {
            cmdlet.WriteError(
                ExceptionHelpers.DuplicatedEntryError(entry: destination, source: Source));
            return;
        }

        destinationEntry = zip.CreateEntry(destination);
        using (Stream sourceStream = sourceEntry.Open())
        using (Stream destinationStream = destinationEntry.Open())
        {
            sourceStream.CopyTo(destinationStream);
        }
        sourceEntry.Delete();
    }
}
