using System;
using System.IO;
using PSCompression.Abstractions.Entries;
using PSCompression.Enum;
using PSCompression.Extensions;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Readers;

namespace PSCompression.FormatHandlers.Rar;

public sealed class RarEntry : EntryBase
{
    public override string? Name { get; protected set; }

    public override string RelativePath { get; }

    public override DateTime LastWriteTime { get; }

    public override long Length { get; internal set; }

    public long CompressedLength { get; internal set; }

    public override EntryType Type { get; }

    public bool IsEncrypted { get; }

    internal override string? FormatDirectoryPath { get; }

    internal RarEntry(IArchiveEntry entry, string source) : base(source)
    {
        Name = Path.GetFileName(entry.Key);
        RelativePath = entry.Key ?? "";
        Type = entry.IsDirectory ? EntryType.Directory : EntryType.Archive;
        Length = entry.Size;
        CompressedLength = entry.CompressedSize;
        LastWriteTime = entry.LastModifiedTime ?? DateTime.MinValue;
        IsEncrypted = entry.IsEncrypted;
        FormatDirectoryPath = Type == EntryType.Directory
            ? $"/{RelativePath.NormalizeEntryPath()}"
            : $"/{Path.GetDirectoryName(RelativePath)?.NormalizeEntryPath()}";
    }

    internal RarEntry(IArchiveEntry entry, Stream stream)
        : this(entry, $"InputStream.{Guid.NewGuid()}")
    {
        stream.Seek(0, SeekOrigin.Begin);
        Stream = stream;
    }

    internal IRarArchive OpenRead()
    {
        if (!FromStream) return RarArchive.OpenArchive(Source);
        Stream.Seek(0, SeekOrigin.Begin);
        return RarArchive.OpenArchive(Stream, new ReaderOptions { LeaveStreamOpen = true });
    }
}
