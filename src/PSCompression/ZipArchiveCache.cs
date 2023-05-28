using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace PSCompression;

internal class ZipArchiveCache : IDisposable
{
    private readonly Dictionary<string, ZipArchive> _cache;

    internal ZipArchiveCache() => _cache = new();

    internal ZipArchive GetOrAdd(ZipEntryBase entry, ZipArchiveMode mode)
    {
        if (!_cache.ContainsKey(entry.Source))
        {
            _cache[entry.Source] = entry.OpenZip(mode);
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
