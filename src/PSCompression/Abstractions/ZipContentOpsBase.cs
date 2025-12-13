using System;

namespace PSCompression.Abstractions;

internal abstract class ZipContentOpsBase<TArchive>(TArchive zip) : IDisposable
    where TArchive : IDisposable
{
    protected byte[]? Buffer { get; set; }

    protected TArchive ZipArchive { get; } = zip;

    private bool _disposed;

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
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
