using System;
using System.Collections.Generic;
using PSCompression.Abstractions;

namespace PSCompression;

internal sealed class ZipArchiveCache<TArchive> : IDisposable
    where TArchive : IDisposable
{
    private readonly Dictionary<string, TArchive> _cache;

    private readonly Func<ZipEntryBase, TArchive> _factory;

    internal ZipArchiveCache(Func<ZipEntryBase, TArchive> factory)
    {
        _cache = new(StringComparer.OrdinalIgnoreCase);
        _factory = factory;
    }

    internal TArchive this[string source] => _cache[source];

    internal void TryAdd(ZipEntryBase entry)
    {
        if (!_cache.ContainsKey(entry.Source))
        {
            _cache[entry.Source] = _factory(entry);
        }
    }

    internal TArchive GetOrCreate(ZipEntryBase entry)
    {
        if (!_cache.ContainsKey(entry.Source))
        {
            _cache[entry.Source] = _factory(entry);
        }

        return _cache[entry.Source];
    }

    public void Dispose()
    {
        foreach (TArchive zip in _cache.Values)
        {
            zip?.Dispose();
        }
    }
}
