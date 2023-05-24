using System;
using System.IO.Compression;

namespace PSCompression;

public enum ZipEntryType
{
    Directory = 0,
    File = 1
}

public abstract class ZipEntryBase
{
    private readonly static string[] _suffix =
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

        return Math.Round(len, 2) + " " + _suffix[index];
    }

    public void RemoveEntry()
    {
        using ZipArchive zip = ZipFile.Open(Source, ZipArchiveMode.Update);
        zip.GetEntry(EntryRelativePath).Delete();
    }

    internal void RemoveEntry(ZipArchive zip)
    {
        zip.GetEntry(EntryRelativePath).Delete();
    }

    internal ZipArchive OpenZip(ZipArchiveMode mode)
    {
        return ZipFile.Open(Source, mode);
    }
}
