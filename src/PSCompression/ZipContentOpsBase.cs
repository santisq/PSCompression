using System;
using System.IO.Compression;

namespace PSCompression;

internal abstract class ZipContentOpsBase(ZipArchive zip) : IDisposable
{
    protected byte[]? _buffer;

    protected ZipArchive _zip = zip;

    protected bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _zip.Dispose();
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
