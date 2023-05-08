using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PSCompression;

public sealed class ZipEntryStream : Stream
{

    public ZipEntryStream(ZipEntryFile entry)
    {

    }

    public override bool CanRead => throw new NotImplementedException();

    public override bool CanSeek => throw new NotImplementedException();

    public override bool CanTimeout => base.CanTimeout;

    public override bool CanWrite => throw new NotImplementedException();

    public override long Length => throw new NotImplementedException();

    public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public override int ReadTimeout { get => base.ReadTimeout; set => base.ReadTimeout = value; }
    public override int WriteTimeout { get => base.WriteTimeout; set => base.WriteTimeout = value; }

    public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
        return base.BeginRead(buffer, offset, count, callback, state);
    }

    public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
        return base.BeginWrite(buffer, offset, count, callback, state);
    }

    public override void Close()
    {
        base.Close();
    }

    public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
        return base.CopyToAsync(destination, bufferSize, cancellationToken);
    }

    public override int EndRead(IAsyncResult asyncResult)
    {
        return base.EndRead(asyncResult);
    }

    public override void EndWrite(IAsyncResult asyncResult)
    {
        base.EndWrite(asyncResult);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override void Flush()
    {
        throw new NotImplementedException();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return base.FlushAsync(cancellationToken);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override object InitializeLifetimeService()
    {
        return base.InitializeLifetimeService();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return base.ReadAsync(buffer, offset, count, cancellationToken);
    }

    public override int ReadByte()
    {
        return base.ReadByte();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        throw new NotImplementedException();
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return base.WriteAsync(buffer, offset, count, cancellationToken);
    }

    public override void WriteByte(byte value)
    {
        base.WriteByte(value);
    }

    protected override WaitHandle CreateWaitHandle()
    {
        return base.CreateWaitHandle();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

    protected override void ObjectInvariant()
    {
        base.ObjectInvariant();
    }
}
