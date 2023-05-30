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
    private readonly static string[] s_suffix =
    {
        "Bytes",
        "Kb",
        "Mb",
        "Gb",
        "Tb",
        "Pb",
        "Eb",
        "Zb",
        "Yb"
    };

    public string Source { get; }

    public string EntryName { get; }

    public string EntryRelativePath { get; }

    public DateTime LastWriteTime { get; }

    public long Length { get; internal set; }

    public long CompressedLength { get; internal set; }

    public string Size => FormatLength(Length);

    public string CompressedSize => FormatLength(CompressedLength);

    protected ZipEntryBase(ZipArchiveEntry entry, string source)
    {
        Source = source;
        EntryName = entry.Name;
        EntryRelativePath = entry.FullName;
        LastWriteTime = entry.LastWriteTime.LocalDateTime;
        Length = entry.Length;
        CompressedLength = entry.CompressedLength;
    }

    private static string FormatLength(long length)
    {
        int index = 0;
        double len = length;

        while (len >= 1024)
        {
            len /= 1024;
            index++;
        }

        return Math.Round(len, 2) + " " + s_suffix[index];
    }

    public void RemoveEntry()
    {
        using ZipArchive zip = ZipFile.Open(Source, ZipArchiveMode.Update);
        zip.GetEntry(EntryRelativePath).Delete();
    }

    internal void RemoveEntry(ZipArchive zip) =>
        zip.GetEntry(EntryRelativePath).Delete();

    internal ZipArchive OpenZip(ZipArchiveMode mode) =>
        ZipFile.Open(Source, mode);

    internal (string, bool) ExtractTo(ZipArchive zip, string destination, bool overwrite)
    {
        destination = Path.GetFullPath(Path.Combine(destination, EntryRelativePath));

        if (string.IsNullOrEmpty(EntryName))
        {
            Directory.CreateDirectory(destination);
            return (destination, false);
        }

        string parent = Path.GetDirectoryName(destination);

        if (!Directory.Exists(parent))
        {
            Directory.CreateDirectory(parent);
        }

        ZipArchiveEntry entry = zip.GetEntry(EntryRelativePath);
        entry.ExtractToFile(destination, overwrite);
        return (destination, true);
    }
}
