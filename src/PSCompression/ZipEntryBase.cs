using System;
using System.IO;
using System.IO.Compression;

namespace PSCompression;

public enum ZipEntryType
{
    Directory = 0,
    Archive = 1
}

public abstract class ZipEntryBase
{
    public string Source { get; }

    public string Name { get; }

    public string RelativePath { get; }

    public DateTime LastWriteTime { get; }

    public long? Length { get; internal set; }

    public long? CompressedLength { get; internal set; }

    public abstract ZipEntryType Type { get; }

    protected ZipEntryBase(ZipArchiveEntry entry, string source)
    {
        Source = source;
        Name = entry.Name;
        RelativePath = entry.FullName;
        LastWriteTime = entry.LastWriteTime.LocalDateTime;
        Length = entry.Length;
        CompressedLength = entry.CompressedLength;
    }

    public void Remove()
    {
        using ZipArchive zip = ZipFile.Open(Source, ZipArchiveMode.Update);
        zip.GetEntry(RelativePath)?.Delete();
    }

    internal void Remove(ZipArchive zip) =>
        zip.GetEntry(RelativePath)?
        .Delete();

    internal abstract string Move(
        string destination,
        ZipArchive zip);

    internal abstract string Rename(
        string newname,
        ZipArchive zip);

    internal ZipArchive Open(ZipArchiveMode mode) =>
        ZipFile.Open(Source, mode);

    internal (string, bool) ExtractTo(
        ZipArchive zip,
        string destination,
        bool overwrite)
    {
        destination = Path.GetFullPath(
            Path.Combine(destination, RelativePath));

        if (string.IsNullOrEmpty(Name))
        {
            Directory.CreateDirectory(destination);
            return (destination, false);
        }

        string parent = Path.GetDirectoryName(destination);

        if (!Directory.Exists(parent))
        {
            Directory.CreateDirectory(parent);
        }

        ZipArchiveEntry entry = zip.GetEntry(RelativePath);
        entry.ExtractToFile(destination, overwrite);
        return (destination, true);
    }

    public FileSystemInfo ExtractTo(string destination, bool overwrite)
    {
        using ZipArchive zip = ZipFile.OpenRead(Source);
        (string path, bool isArchive) = ExtractTo(zip, destination, overwrite);

        if (isArchive)
        {
            return new FileInfo(path);
        }

        return new DirectoryInfo(path);
    }

    public override string ToString() => RelativePath;
}
