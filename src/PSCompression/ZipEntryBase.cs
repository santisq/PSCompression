using System;
using System.IO;
using System.IO.Compression;
using PSCompression.Extensions;
using PSCompression.Exceptions;

namespace PSCompression;

public abstract class ZipEntryBase(ZipArchiveEntry entry, string source)
{
    protected string? _formatDirectoryPath;

    protected Stream? _stream;

    internal bool FromStream { get => _stream is not null; }

    internal abstract string FormatDirectoryPath { get; }

    public string Source { get; } = source;

    public string Name { get; protected set; } = entry.Name;

    public string RelativePath { get; } = entry.FullName;

    public DateTime LastWriteTime { get; } = entry.LastWriteTime.LocalDateTime;

    public long Length { get; internal set; } = entry.Length;

    public long CompressedLength { get; internal set; } = entry.CompressedLength;

    public abstract ZipEntryType Type { get; }

    protected ZipEntryBase(ZipArchiveEntry entry, Stream? stream)
        : this(entry, $"InputStream.{Guid.NewGuid()}")
    {
        _stream = stream;
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
        _stream is null
            ? ZipFile.Open(Source, mode)
            : new ZipArchive(_stream, mode, true);

    public FileSystemInfo ExtractTo(string destination, bool overwrite)
    {
        using ZipArchive zip = ZipFile.OpenRead(Source);
        (string path, bool isArchive) = this.ExtractTo(zip, destination, overwrite);

        if (isArchive)
        {
            return new FileInfo(path);
        }

        return new DirectoryInfo(path);
    }

    public override string ToString() => RelativePath;
}
