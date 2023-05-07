using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

public enum ZipEntryType
{
    Directory = 0,
    File = 1
}

public abstract class ZipEntryBase
{
    private readonly ZipArchiveEntry _entry;

    private readonly static string[] _suffix;

    static ZipEntryBase() =>
        _suffix = new string[9] { "Bytes", "Kb", "Mb", "Gb", "Tb", "Pb", "Eb", "Zb", "Yb" };

    public string Source { get; }

    public string EntryName => _entry.Name;

    public DateTime LastWriteTime => _entry.LastWriteTime.LocalDateTime;

    public string EntryRelativePath => _entry.FullName;

    public long Length => _entry.Length;

    public long CompressedLength => _entry.CompressedLength;

    public string Size => FormatLength(Length);

    public string CompressedSize => FormatLength(CompressedLength);

    protected ZipEntryBase(ZipArchiveEntry entry, string source)
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
}

public sealed class ZipEntryFile : ZipEntryBase, IDisposable
{
    private ZipArchive? _zipStream;

    private Stream? _entryStream;

    public ZipEntryType EntryType => ZipEntryType.File;

    internal ZipEntryFile(ZipArchiveEntry entry, string source) :
        base(entry, source)
    { }

    internal void OpenRead()
    {
        _zipStream = ZipFile.OpenRead(Source);
        _entryStream = _zipStream.GetEntry(EntryRelativePath).Open();
    }

    private void ValidateStreamOpen()
    {
        if(_zipStream is null || _entryStream is null)
        {
            throw new ArgumentNullException("Stream is not opened.");
        }
    }

    internal void ReadLines(Encoding encoding, PSCmdlet cmdlet, bool detectEncoding = true)
    {
        ValidateStreamOpen();
        using StreamReader reader = new(_entryStream, encoding, detectEncoding);

        while (!reader.EndOfStream)
        {
            cmdlet.WriteObject(reader.ReadLine());
        }
    }

    internal string ReadToEnd(Encoding encoding, bool detectEncoding = true)
    {
        ValidateStreamOpen();
        using StreamReader reader = new(_entryStream, encoding, detectEncoding);
        return reader.ReadToEnd();
    }

    internal void ReadLines(PSCmdlet cmdlet) => ReadLines(Encoding.UTF8, cmdlet: cmdlet);

    internal string ReadToEnd() => ReadToEnd(Encoding.UTF8);

    public void Dispose()
    {
        _entryStream?.Dispose();
        _zipStream?.Dispose();
    }
}

public sealed class ZipEntryDirectory : ZipEntryBase
{
    public ZipEntryType EntryType => ZipEntryType.Directory;

    internal ZipEntryDirectory(ZipArchiveEntry entry, string source) :
        base(entry, source)
    { }
}
