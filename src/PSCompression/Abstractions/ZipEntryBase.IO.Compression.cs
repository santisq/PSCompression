using System.IO;
using System.IO.Compression;
using PSCompression.Exceptions;
using PSCompression.Extensions;

namespace PSCompression.Abstractions;

public abstract partial class ZipEntryBase
{
    public ZipArchive OpenRead() =>
        FromStream ? new ZipArchive(_stream) : ZipFile.OpenRead(Source);

    public ZipArchive OpenWrite()
    {
        this.ThrowIfFromStream();
        return ZipFile.Open(Source, ZipArchiveMode.Update);
    }

    public void Remove()
    {
        this.ThrowIfFromStream();

        using ZipArchive zip = ZipFile.Open(
            Source,
            ZipArchiveMode.Update);

        zip.ThrowIfNotFound(
            path: RelativePath,
            source: Source,
            out ZipArchiveEntry entry);

        entry.Delete();
    }

    internal void Remove(ZipArchive zip)
    {
        this.ThrowIfFromStream();

        zip.ThrowIfNotFound(
            path: RelativePath,
            source: Source,
            out ZipArchiveEntry entry);

        entry.Delete();
    }

    internal static string Move(
        string sourceRelativePath,
        string destination,
        string sourceZipPath,
        ZipArchive zip)
    {
        zip.ThrowIfNotFound(
            path: sourceRelativePath,
            source: sourceZipPath,
            entry: out ZipArchiveEntry sourceEntry);

        zip.ThrowIfDuplicate(
            path: destination,
            source: sourceZipPath);

        destination.ThrowIfInvalidPathChar();

        ZipArchiveEntry destinationEntry = zip.CreateEntry(destination);
        using (Stream sourceStream = sourceEntry.Open())
        using (Stream destinationStream = destinationEntry.Open())
        {
            sourceStream.CopyTo(destinationStream);
        }

        sourceEntry.Delete();
        return destination;
    }

    internal ZipArchive OpenZip(ZipArchiveMode mode) =>
        FromStream
            ? new ZipArchive(_stream, mode, true)
            : ZipFile.Open(Source, mode);

    public FileSystemInfo ExtractTo(string destination, bool overwrite)
    {
        using ZipArchive zip = FromStream
            ? new ZipArchive(_stream, ZipArchiveMode.Read, leaveOpen: true)
            : ZipFile.OpenRead(Source);

        return ExtractTo(destination, overwrite, zip);
    }

    internal FileSystemInfo ExtractTo(
        string destination,
        bool overwrite,
        ZipArchive zip)
    {
        destination = Path.GetFullPath(
            Path.Combine(destination, RelativePath));

        if (Type == EntryType.Directory)
        {
            DirectoryInfo dir = new(destination);
            dir.Create(overwrite);
            return dir;
        }

        FileInfo file = new(destination);
        file.Directory?.Create();

        if (zip.TryGetEntry(RelativePath, out ZipArchiveEntry? entry))
        {
            entry.ExtractToFile(destination, overwrite);
        }

        return file;
    }
}
