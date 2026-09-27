using System;
using System.Collections.Generic;
using PSCompression.Abstractions;

namespace PSCompression.FormatHandlers.Common;

internal sealed class ArchiveCache<TArchive, TEntry> : IDisposable
    where TArchive : IDisposable
    where TEntry : EntryBase
{
    private readonly Dictionary<string, TArchive> _cache;

    private readonly Func<TEntry, TArchive> _factory;

    internal ArchiveCache(Func<TEntry, TArchive> factory)
    {
        _cache = new(StringComparer.OrdinalIgnoreCase);
        _factory = factory;
    }

    internal TArchive this[string source] => _cache[source];

    internal void TryAdd(TEntry entry)
    {
        if (!_cache.ContainsKey(entry.Source))
            _cache[entry.Source] = _factory(entry);
    }

    internal TArchive GetOrCreate(TEntry entry)
    {
        if (!_cache.TryGetValue(entry.Source, out TArchive? archive))
        {
            archive = _factory(entry);
            _cache[entry.Source] = archive;
        }

        return archive;
    }

    public void Dispose()
    {
        foreach (TArchive archive in _cache.Values)
            archive?.Dispose();
    }
}
