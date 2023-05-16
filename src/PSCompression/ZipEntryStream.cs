using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;

namespace PSCompression;

public sealed class ZipEntryStream : Stream
{
    public Stream EntryStream { get; }

    public ZipArchive ZipStream { get; }

    public bool Disposed { get; private set; }

    public ZipEntryStream(ZipEntryFile entry, ZipArchiveMode mode)
    {
        ZipStream = ZipFile.Open(entry.Source, mode);
        EntryStream = ZipStream.GetEntry(entry.EntryRelativePath).Open();
    }

    public override bool CanRead => EntryStream.CanRead;

    public override bool CanSeek => EntryStream.CanSeek;

    public override bool CanTimeout => EntryStream.CanTimeout;

    public override bool CanWrite => EntryStream.CanWrite;

    public override long Length => EntryStream.Length;

    public override long Position
    {
        get => EntryStream.Position;
        set => EntryStream.Position = value;
    }

    public override int ReadTimeout
    {
        get => EntryStream.ReadTimeout;
        set => EntryStream.ReadTimeout = value;
    }

    public override int WriteTimeout
    {
        get => EntryStream.WriteTimeout;
        set => EntryStream.WriteTimeout = value;
    }

    public override IAsyncResult BeginRead(
        byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
            EntryStream.BeginRead(buffer, offset, count, callback, state);

    public override IAsyncResult BeginWrite(
        byte[] buffer, int offset, int count, AsyncCallback callback, object state) =>
            EntryStream.BeginWrite(buffer, offset, count, callback, state);

    public override Task CopyToAsync(
        Stream destination, int bufferSize, CancellationToken cancellationToken) =>
            EntryStream.CopyToAsync(destination, bufferSize, cancellationToken);

    public override int EndRead(IAsyncResult asyncResult) =>
        EntryStream.EndRead(asyncResult);

    public override void EndWrite(IAsyncResult asyncResult) =>
        EntryStream.EndWrite(asyncResult);

    public override bool Equals(object obj) => EntryStream.Equals(obj);

    public override void Flush() => EntryStream.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) =>
        EntryStream.FlushAsync(cancellationToken);

    public override int GetHashCode() => EntryStream.GetHashCode();

    public override object InitializeLifetimeService() =>
        EntryStream.InitializeLifetimeService();

    public override int Read(byte[] buffer, int offset, int count) =>
        EntryStream.Read(buffer, offset, count);

    public override Task<int> ReadAsync(
        byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            EntryStream.ReadAsync(buffer, offset, count, cancellationToken);

    public override int ReadByte() => EntryStream.ReadByte();

    public override long Seek(long offset, SeekOrigin origin) =>
        EntryStream.Seek(offset, origin);

    public override void SetLength(long value) => EntryStream.SetLength(value);

    public override string ToString() => EntryStream.ToString();

    public override void Write(byte[] buffer, int offset, int count) =>
        EntryStream.Write(buffer, offset, count);

    public override Task WriteAsync(
        byte[] buffer, int offset, int count, CancellationToken cancellationToken) =>
            EntryStream.WriteAsync(buffer, offset, count, cancellationToken);

    public override void WriteByte(byte value) => EntryStream.WriteByte(value);

    public override void Close()
    {
        EntryStream?.Close();
        ZipStream?.Dispose();
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        ZipStream?.Dispose();
        Disposed = true;
    }
}
