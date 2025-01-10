using System;
using System.IO.Compression;

namespace PSCompression;

internal abstract class ZipContentOpsBase(ZipArchive zip) : IDisposable
{
    protected ZipArchive _zip = zip;

    protected byte[]? _buffer;

    public bool Disposed { get; internal set; }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !Disposed)
        {
            _zip?.Dispose();
            Disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
