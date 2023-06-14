using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace PSCompression;

internal class ZipArchiveCache : IDisposable
{
    private readonly Dictionary<string, ZipArchive> _cache;

    private readonly ZipArchiveMode _mode = ZipArchiveMode.Read;

    internal ZipArchiveCache() => _cache = new();

    internal ZipArchiveCache(ZipArchiveMode mode)
    {
        _cache = new Dictionary<string, ZipArchive>();
        _mode = mode;
    }

    internal ZipArchive GetOrAdd(ZipEntryBase entry)
    {
        if (!_cache.ContainsKey(entry.Source))
        {
            _cache[entry.Source] = entry.OpenZip(_mode);
        }

        return _cache[entry.Source];
    }

    public void Dispose()
    {
        foreach (ZipArchive zip in _cache.Values)
        {
            zip?.Dispose();
        }
    }
}
