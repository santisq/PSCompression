using System.IO.Compression;

namespace PSCompression;

internal sealed class ZipArchiveCache : ZipCacheBase<ZipArchive>
{
    internal ZipArchive GetOrAdd(ZipEntryBase entry, ZipArchiveMode mode)
    {
        if (!_cache.ContainsKey(entry.Source))
        {
            _cache[entry.Source] = entry.OpenZip(mode);
        }

        return _cache[entry.Source];
    }

    public override void Dispose()
    {
        foreach (ZipArchive zip in _cache.Values)
        {
            zip?.Dispose();
        }
    }
}
