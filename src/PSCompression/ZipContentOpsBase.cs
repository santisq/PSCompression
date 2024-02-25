using System;
using System.IO.Compression;

namespace PSCompression;

internal abstract class ZipContentOpsBase : IDisposable
{
    public virtual ZipArchive ZipArchive { get; } = default!;

    protected byte[]? _buffer;

    public bool Disposed { get; internal set; }

    protected ZipContentOpsBase(ZipArchive zip) =>
        ZipArchive = zip;

    protected ZipContentOpsBase()
    {

    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !Disposed)
        {
            ZipArchive?.Dispose();
            Disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
