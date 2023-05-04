using System;
using System.IO;
using System.IO.Compression;

namespace PSCompression;

public class ZipEntry
{
    private readonly ZipArchiveEntry _instance;

    public string Name => _instance.Name;

    public DateTimeOffset LastWriteTime => _instance.LastWriteTime;

    public string EntryRelativePath => _instance.FullName;

    internal ZipEntry(ZipArchiveEntry entry)
    {
        _instance = entry;
    }

    // public Stream
}
