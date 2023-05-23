using System;
using System.IO.Compression;

namespace PSCompression;

internal abstract class ZipContentOpsBase : IDisposable
{
    internal string Source { get; }

    protected abstract ZipArchive ZipArchive { get; }

    protected byte[]? _buffer;

    protected bool _disposed;

    protected ZipContentOpsBase(string source)
    {
        Source = source;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            ZipArchive.Dispose();
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
