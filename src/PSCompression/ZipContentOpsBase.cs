using System;
using System.IO.Compression;

namespace PSCompression;

internal abstract class ZipContentOpsBase : IDisposable
{
    public virtual ZipArchive ZipArchive { get; } = default!;

    protected byte[]? _buffer;

    protected bool _disposed;

    protected ZipContentOpsBase(ZipArchive zip) =>
        ZipArchive = zip;

    protected ZipContentOpsBase()
    { }

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
