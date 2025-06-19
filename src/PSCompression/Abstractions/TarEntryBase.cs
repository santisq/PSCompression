using System;
using System.IO;
using ICSharpCode.SharpZipLib.Tar;
using PSCompression.Extensions;

namespace PSCompression.Abstractions;

public abstract class TarEntryBase(TarEntry entry, string source) : EntryBase(source)
{
    public override string Name { get; protected set; } = Path.GetFileName(entry.Name);

    public override string RelativePath { get; } = entry.Name;

    public override DateTime LastWriteTime { get; } = entry.ModTime;

    public override long Length { get; internal set; } = entry.Size;

    protected TarEntryBase(TarEntry entry, Stream? stream)
        : this(entry, $"InputStream.{Guid.NewGuid()}")
    {
        _stream = stream;
    }

    internal FileSystemInfo ExtractTo(
        string destination,
        bool overwrite)
    {
        destination = Path.GetFullPath(
            Path.Combine(destination, RelativePath));

        if (this is not TarEntryFile entryFile)
        {
            Directory.CreateDirectory(destination);
            return new DirectoryInfo(destination);
        }

        string parent = destination.GetParent();
        if (!Directory.Exists(parent))
        {
            Directory.CreateDirectory(parent);
        }

        using FileStream destStream = File.Open(
            destination,
            overwrite ? FileMode.Create : FileMode.CreateNew,
            FileAccess.Write);

        entryFile.GetContentStream(destStream);
        return new FileInfo(destination);
    }
}
