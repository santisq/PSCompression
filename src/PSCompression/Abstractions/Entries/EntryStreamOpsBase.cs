using System;
using System.IO;

namespace PSCompression.Abstractions.Entries;

internal abstract class EntryStreamOpsBase(Stream stream) : IDisposable
{
    private bool _disposed;

    protected Stream Stream { get; } = stream;

    protected virtual void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            Stream.Dispose();
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
    }
}
