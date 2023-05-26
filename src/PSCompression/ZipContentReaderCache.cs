namespace PSCompression;

internal sealed class ZipContentReaderCache : ZipCacheBase<ZipContentReader>
{
    internal ZipContentReader GetOrAdd(ZipEntryFile entry)
    {
        if (!_cache.ContainsKey(entry.Source))
        {
            _cache[entry.Source] = new ZipContentReader(entry.Source);
        }

        return _cache[entry.Source];
    }

    public override void Dispose()
    {
        foreach (ZipContentReader reader in _cache.Values)
        {
            reader?.Dispose();
        }
    }
}
