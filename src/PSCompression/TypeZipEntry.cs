using System;
using System.IO.Compression;

namespace PSCompression;

public class ZipEntry
{
    internal static string[] _suffix;

    static ZipEntry() =>
        _suffix = new string[] { "Bytes", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb" };

    public string Source { get; }

    public string EntryName { get; }

    public DateTimeOffset LastWriteTime { get; }

    public string EntryRelativePath { get; }

    public long Length { get; }

    public long CompressedLength { get; }

    public string Size => FormatLength(Length);

    public string CompressedSize => FormatLength(CompressedLength);

    public string EntryType => string.IsNullOrEmpty(EntryName) ? "Directory" : "File";

    internal ZipEntry(ZipArchiveEntry entry, string source)
    {
        Source = source;
        EntryName = entry.Name;
        EntryRelativePath = entry.FullName;
        LastWriteTime = entry.LastWriteTime;
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

        return string.Format("{0} {1}", Math.Round(len, 2), _suffix[index]);
    }
    // public Stream
}

