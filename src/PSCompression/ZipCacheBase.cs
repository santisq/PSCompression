using System;
using System.Collections.Generic;

namespace PSCompression;

internal abstract class ZipCacheBase<T> : IDisposable
{
    protected readonly Dictionary<string, T> _cache;

    protected ZipCacheBase() => _cache = new();

    public abstract void Dispose();
}
