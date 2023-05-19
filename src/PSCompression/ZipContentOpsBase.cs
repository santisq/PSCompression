using System;
using System.IO.Compression;

namespace PSCompression;

internal abstract class ZipContentOpsBase : IDisposable
{
    internal string Source { get; }

    internal ZipArchive ZipArchive { get; }

    private bool _disposed;

    internal byte[]? _buffer;

    protected ZipContentOpsBase(string source, ZipArchiveMode mode)
    {
        ZipArchive = ZipFile.Open(source, mode);
        Source = source;
    }

    protected virtual void Dispose(bool disposing)
    {
        if(!_disposed)
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
