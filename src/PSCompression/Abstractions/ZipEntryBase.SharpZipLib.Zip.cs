using System;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace PSCompression.Abstractions;

public abstract partial class ZipEntryBase(ZipEntry entry, string source)
    : EntryBase(source)
{
    public override string? Name { get; protected set; }

    public override string RelativePath { get; } = entry.Name;

    public override DateTime LastWriteTime { get; } = entry.DateTime;

    public override long Length { get; internal set; } = entry.Size;

    public long CompressedLength { get; internal set; } = entry.CompressedSize;

    protected ZipEntryBase(ZipEntry entry, Stream? stream)
        : this(entry, $"InputStream.{Guid.NewGuid()}")
    {
        _stream = stream;
    }
}
