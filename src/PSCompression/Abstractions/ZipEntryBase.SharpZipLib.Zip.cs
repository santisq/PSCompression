using System;
using System.IO;
using System.Security;
using ICSharpCode.SharpZipLib.Zip;
using PSCompression.Extensions;

namespace PSCompression.Abstractions;

public abstract partial class ZipEntryBase(ZipEntry entry, string source)
    : EntryBase(source)
{
    public override string? Name { get; protected set; }

    public override string RelativePath { get; } = entry.Name;

    public override DateTime LastWriteTime { get; } = entry.DateTime;

    public override long Length { get; internal set; } = entry.Size;

    public long CompressedLength { get; internal set; } = entry.CompressedSize;

    public bool IsEncrypted { get; } = entry.IsCrypted;

    public int AESKeySize { get; } = entry.AESKeySize;

    public CompressionMethod CompressionMethod { get; } = entry.CompressionMethod;

    public string Comment { get; } = entry.Comment;

    protected ZipEntryBase(ZipEntry entry, Stream? stream)
        : this(entry, $"InputStream.{Guid.NewGuid()}")
    {
        _stream = stream;
    }

    internal ZipFile OpenRead(SecureString? password)
    {
        ZipFile zip = FromStream ? new(_stream, leaveOpen: true) : new(Source);
        if (password?.Length > 0)
        {
            zip.Password = password.AsPlainText();
        }

        return zip;
    }

    public FileSystemInfo ExtractTo(
        string destination,
        bool overwrite,
        SecureString? password = null)
    {
        using ZipFile zip = FromStream
            ? new(_stream, leaveOpen: true)
            : new(Source);

        if (password?.Length > 0)
        {
            zip.Password = password.AsPlainText();
        }

        return ExtractTo(destination, overwrite, zip);
    }

    internal FileSystemInfo ExtractTo(
        string destination,
        bool overwrite,
        ZipFile zip)
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

        if (zip.TryGetEntry(RelativePath, out ZipEntry? entry))
        {
            using Stream source = zip.GetInputStream(entry);
            using FileStream fs = file.OpenWrite();
            source.CopyTo(fs);
        }

        return file;
    }
}
