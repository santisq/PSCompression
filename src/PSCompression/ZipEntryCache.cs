using System;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using PSCompression.Abstractions;
using PSCompression.Extensions;

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
            using ZipFile zip = new(entry.Key);
            foreach ((string path, EntryType type) in entry.Value)
            {
                if (!zip.TryGetEntry(path, out ZipEntry? zipEntry))
                {
                    continue;
                }

                if (type == EntryType.Archive)
                {
                    yield return new ZipEntryFile(zipEntry, entry.Key);
                    continue;
                }

                yield return new ZipEntryDirectory(zipEntry, entry.Key);
            }
        }
    }
}
