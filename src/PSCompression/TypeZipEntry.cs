using System;
using System.IO.Compression;

namespace PSCompression;

public class ZipEntry
{
    private readonly ZipArchiveEntry _entry;

    private readonly static string[] _suffix;

    static ZipEntry() =>
        _suffix = new string[9] { "Bytes", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb" };

    public string Source  { get; }

    public string EntryName => _entry.Name;

    public DateTimeOffset LastWriteTime => _entry.LastWriteTime;

    public string EntryRelativePath => _entry.FullName;

    public long Length => _entry.Length;

    public long CompressedLength => _entry.CompressedLength;

    public string Size => FormatLength(Length);

    public string CompressedSize => FormatLength(CompressedLength);

    public string EntryType => string.IsNullOrEmpty(EntryName) ? "Directory" : "File";

    internal ZipEntry(ZipArchiveEntry entry, string source)
    {
        _entry = entry;
        Source = source;
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

