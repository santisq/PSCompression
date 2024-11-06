using System;
using System.Collections.Generic;
using System.IO.Compression;

namespace PSCompression;

public sealed class ZipEntryCache
{
    private readonly Dictionary<string, List<PathWithType>> _cache;

    internal ZipEntryCache() => _cache = new(StringComparer.InvariantCultureIgnoreCase);

    internal List<PathWithType> WithSource(string source)
    {
        if (!_cache.ContainsKey(source))
        {
            _cache[source] = [];
        }

        return _cache[source];
    }

    internal void Add(string source, PathWithType pathWithType) =>
        WithSource(source).Add(pathWithType);

    internal ZipEntryCache AddRange(IEnumerable<(string, PathWithType)> values)
    {
        foreach ((string source, PathWithType pathWithType) in values)
        {
            Add(source, pathWithType);
        }

        return this;
    }

    internal IEnumerable<ZipEntryBase> GetEntries()
    {
        foreach (var entry in _cache)
        {
            using ZipArchive zip = ZipFile.OpenRead(entry.Key);
            foreach ((string path, ZipEntryType type) in entry.Value)
            {
                if (type is ZipEntryType.Archive)
                {
                    yield return new ZipEntryFile(zip.GetEntry(path), entry.Key);
                    continue;
                }

                yield return new ZipEntryDirectory(zip.GetEntry(path), entry.Key);
            }
        }
    }
}
