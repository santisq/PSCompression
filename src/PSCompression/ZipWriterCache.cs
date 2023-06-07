using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Text;

namespace PSCompression;

internal class ZipWriterCache : IDisposable
{
    private readonly Dictionary<string, ZipContentWriter> _cache;

    private readonly ZipArchive _stream;

    private readonly Encoding _encoding;

    internal ZipWriterCache(ZipArchive zip, Encoding encoding)
    {
        _cache = new();
        _stream = zip;
        _encoding = encoding;
    }

    internal ZipContentWriter GetOrAdd(ZipEntryFile entry)
    {
        if (!_cache.ContainsKey(entry.EntryRelativePath))
        {
            _cache[entry.EntryRelativePath] = new ZipContentWriter(_stream, entry, _encoding);
        }

        return _cache[entry.EntryRelativePath];
    }

    public void Dispose()
    {
        foreach (ZipContentWriter writer in _cache.Values)
        {
            writer?.Dispose();
        }

        _stream?.Dispose();
    }
}
