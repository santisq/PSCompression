using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

public enum ZipEntryType
{
    Directory = 0,
    File      = 1
}

public class ZipEntryBase
{
    private readonly ZipArchiveEntry _entry;

    private readonly static string[] _suffix;

    static ZipEntryBase() =>
        _suffix = new string[9] { "Bytes", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb" };

    public string Source { get; }

    public string EntryName => _entry.Name;

    public DateTimeOffset LastWriteTime => _entry.LastWriteTime;

    public string EntryRelativePath => _entry.FullName;

    public long Length => _entry.Length;

    public long CompressedLength => _entry.CompressedLength;

    public string Size => FormatLength(Length);

    public string CompressedSize => FormatLength(CompressedLength);

    public ZipEntryType EntryType => string.IsNullOrEmpty(EntryName) ?
        ZipEntryType.Directory : ZipEntryType.File;

    internal ZipEntryBase(ZipArchiveEntry entry, string source)
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

    private ZipArchive OpenRead() => ZipFile.OpenRead(Source);

    private Stream GetStream(ZipArchive zip) => zip.GetEntry(EntryRelativePath).Open();

    internal void ReadLines(Encoding encoding, PSCmdlet cmdlet, bool detectEncoding = true)
    {
        using ZipArchive zip = OpenRead();
        using Stream entryStream = GetStream(zip);
        using StreamReader reader = new(entryStream, encoding, detectEncoding);

        while (!reader.EndOfStream)
        {
            cmdlet.WriteObject(reader.ReadLine());
        }
    }

    internal void ReadLines(PSCmdlet cmdlet) => ReadLines(Encoding.UTF8, cmdlet: cmdlet);

    internal string ReadToEnd(Encoding encoding, bool detectEncoding = true)
    {
        using ZipArchive zip = OpenRead();
        using Stream entryStream = GetStream(zip);
        using StreamReader reader = new(entryStream, encoding, detectEncoding);
        return reader.ReadToEnd();
    }

    internal string ReadToEnd() => ReadToEnd(Encoding.UTF8);

    public void ExtractToFile(string destinationFileName, bool overwrite)
    {

    }
}
