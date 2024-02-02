using System.Collections.Generic;
using System.IO.Compression;

namespace PSCompression;

public sealed class ZipEntryCache
{
    private readonly Dictionary<string, List<(string, ZipEntryType)>> _cache;

    internal ZipEntryCache() => _cache = new();

    internal void Add(
        string source,
        string path,
        ZipEntryType type)
    {
        if (!_cache.ContainsKey(source))
        {
            _cache[source] = new();
        }

        _cache[source].Add((path, type));
    }

    internal IEnumerable<ZipEntryBase> GetEntries()
    {
        foreach (KeyValuePair<string, List<(string, ZipEntryType)>> entry in _cache)
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
