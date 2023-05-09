using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace PSCompression;

public sealed class ZipEntryStream : Stream
{
    private readonly ZipArchive _zipStream;

    private readonly Stream _entryStream;

    public bool Disposed { get; private set; }

    public ZipEntryStream(ZipEntryFile entry, ZipArchiveMode mode)
    {
        _zipStream = ZipFile.Open(entry.Source, mode);
        _entryStream = _zipStream.GetEntry(entry.EntryRelativePath).Open();
    }

    public override bool CanRead => _entryStream.CanRead;

    public override bool CanSeek => _entryStream.CanSeek;

    public override bool CanTimeout => _entryStream.CanTimeout;

    public override bool CanWrite => _entryStream.CanWrite;

    public override long Length => _entryStream.Length;

    public override long Position
    {
        get => _entryStream.Position;
        set => _entryStream.Position = value;
    }

    public override int ReadTimeout
    {
        get => _entryStream.ReadTimeout;
        set => _entryStream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
        get => _entryStream.WriteTimeout;
        set => _entryStream.WriteTimeout = value;
    }

    public override IAsyncResult BeginRead(
        byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
            _entryStream.BeginRead(buffer, offset, count, callback, state);

    public override IAsyncResult BeginWrite(
        byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
            _entryStream.BeginWrite(buffer, offset, count, callback, state);

    public override Task CopyToAsync(
        Stream destination, int bufferSize, CancellationToken cancellationToken) =>
            _entryStream.CopyToAsync(destination, bufferSize, cancellationToken);

    public override int EndRead(IAsyncResult asyncResult) =>
        _entryStream.EndRead(asyncResult);

    public override void EndWrite(IAsyncResult asyncResult) =>
        _entryStream.EndWrite(asyncResult);

    public override bool Equals(object obj) => _entryStream.Equals(obj);

    public override void Flush() => _entryStream.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) =>
        _entryStream.FlushAsync(cancellationToken);

    public override int GetHashCode() => _entryStream.GetHashCode();

    public override object InitializeLifetimeService() =>
        _entryStream.InitializeLifetimeService();

    public override int Read(byte[] buffer, int offset, int count) =>
        _entryStream.Read(buffer, offset, count);

    public override Task<int> ReadAsync(
        byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            _entryStream.ReadAsync(buffer, offset, count, cancellationToken);

    public override int ReadByte() => _entryStream.ReadByte();

    public override long Seek(long offset, SeekOrigin origin) =>
        _entryStream.Seek(offset, origin);

    public override void SetLength(long value) => _entryStream.SetLength(value);

    public override string ToString() => _entryStream.ToString();

    public override void Write(byte[] buffer, int offset, int count) =>
        _entryStream.Write(buffer, offset, count);

    public override Task WriteAsync(
        byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            _entryStream.WriteAsync(buffer, offset, count, cancellationToken);

    public override void WriteByte(byte value) => _entryStream.WriteByte(value);

    public override void Close()
    {
        _entryStream?.Close();
        _zipStream?.Dispose();
    }

    protected override void Dispose(bool disposing)
    {
        if(!disposing)
        {
            return;
        }

        _zipStream?.Dispose();
        Disposed = true;
    }
}
