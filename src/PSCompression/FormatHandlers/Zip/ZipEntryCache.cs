using System;
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip;
using PSCompression.Abstractions.Entries;
using PSCompression.Enum;
using PSCompression.Extensions;

namespace PSCompression.FormatHandlers.Zip;

internal sealed class ZipEntryCache
{
    private readonly Dictionary<string, List<PathWithType>> _cache = new(
        StringComparer.InvariantCultureIgnoreCase);

    internal List<PathWithType> WithSource(string source)
    {
        if (!_cache.TryGetValue(source, out List<PathWithType>? value))
        {
            value = [];
            _cache[source] = value;
        }

        return value;
    }

    internal void Add(string source, PathWithType pathWithType) =>
        WithSource(source).Add(pathWithType);

    internal ZipEntryCache AddRange(IEnumerable<(string, PathWithType)> values)
    {
        foreach ((string source, PathWithType pathWithType) in values)
            Add(source, pathWithType);

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
                    continue;

                yield return type == EntryType.Archive
                    ? new ZipEntryFile(zipEntry, entry.Key)
                    : new ZipEntryDirectory(zipEntry, entry.Key);
            }
        }
    }
}

internal record struct EntryWithPath(ZipEntryBase ZipEntry, string Path);

internal record struct PathWithType(string Path, EntryType EntryType);
