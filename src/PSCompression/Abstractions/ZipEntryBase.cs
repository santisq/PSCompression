using System;
using System.IO;
using System.IO.Compression;
using PSCompression.Exceptions;

namespace PSCompression.Abstractions;

public abstract class ZipEntryBase(ZipArchiveEntry entry, string source) : EntryBase(source)
{
    public override string Name { get; protected set; } = entry.Name;

    public override string RelativePath { get; } = entry.FullName;

    public override DateTime LastWriteTime { get; } = entry.LastWriteTime.LocalDateTime;

    public override long Length { get; internal set; } = entry.Length;

    public long CompressedLength { get; internal set; } = entry.CompressedLength;

    protected ZipEntryBase(ZipArchiveEntry entry, Stream? stream)
        : this(entry, $"InputStream.{Guid.NewGuid()}")
    {
        _stream = stream;
    }

    public ZipArchive OpenRead() => FromStream
        ? new ZipArchive(_stream)
        : ZipFile.OpenRead(Source);

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
        using ZipArchive zip = _stream is not null
            ? new ZipArchive(_stream, ZipArchiveMode.Read, leaveOpen: true)
            : ZipFile.OpenRead(Source);

        return ExtractTo(destination, overwrite, zip);
    }

    internal FileSystemInfo ExtractTo(
        string destination,
        bool overwrite,
        ZipArchive zip)
    {
        destination = Path.GetFullPath(Path.Combine(destination, RelativePath));
        if (Type is EntryType.Directory)
        {
            Directory.CreateDirectory(destination);
            return new DirectoryInfo(destination);
        }

        string parent = Path.GetDirectoryName(destination);
        if (!Directory.Exists(parent))
        {
            Directory.CreateDirectory(parent);
        }

        ZipArchiveEntry entry = zip.GetEntry(RelativePath);
        entry.ExtractToFile(destination, overwrite);
        return new FileInfo(destination);
    }
}
