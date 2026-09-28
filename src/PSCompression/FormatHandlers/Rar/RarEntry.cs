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
    private static readonly ReaderOptions s_readerOptions = new() { LeaveStreamOpen = true };

    public override string? Name { get; protected set; }

    public override string RelativePath { get; }

    public override DateTime? LastWriteTime { get; }

    public override long Length { get; internal set; }

    public long CompressedLength { get; internal set; }

    public override EntryType Type { get; }

    public bool IsEncrypted { get; }

    internal override string? FormatDirectoryPath { get; }

    internal RarEntry(IArchiveEntry entry, string source) : base(source)
    {
        Name = Path.GetFileName(entry.Key);
        RelativePath = $"{entry.Key}";
        Length = entry.Size;
        CompressedLength = entry.CompressedSize;
        LastWriteTime = entry.LastModifiedTime;
        IsEncrypted = entry.IsEncrypted;
        (Type, FormatDirectoryPath) = entry.IsDirectory
            ? (EntryType.Directory, $"/{RelativePath.NormalizeEntryPath()}")
            : (EntryType.Archive, $"/{Path.GetDirectoryName(RelativePath)?.NormalizeEntryPath()}");
    }

    internal RarEntry(IArchiveEntry entry, Stream stream)
        : this(entry, $"InputStream.{Guid.NewGuid()}")
    {
        Stream = stream;
    }

    internal IRarArchive OpenRead()
    {
        if (!FromStream) return RarArchive.OpenArchive(Source);
        Stream.Position = 0;
        return RarArchive.OpenArchive(Stream, s_readerOptions);
    }
}
