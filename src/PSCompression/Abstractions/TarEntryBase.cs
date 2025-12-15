using System;
using System.IO;
using ICSharpCode.SharpZipLib.Tar;
using PSCompression.Extensions;

namespace PSCompression.Abstractions;

public abstract class TarEntryBase(TarEntry entry, string source) : EntryBase(source)
{
    public override string? Name { get; protected set; }

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
            DirectoryInfo dir = new(destination);
            dir.Create(overwrite);
            return dir;
        }

        FileInfo file = new(destination);
        file.Directory?.Create();

        using FileStream destStream = file.Open(
            overwrite ? FileMode.Create : FileMode.CreateNew,
            FileAccess.Write);

        entryFile.GetContentStream(destStream);
        return file;
    }
}
