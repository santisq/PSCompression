using System;
using System.IO;
using System.IO.Compression;

namespace PSCompression;

internal sealed class ZlibStream : Stream
{
    private readonly Stream _outputStream;

    private readonly DeflateStream _deflateStream;

    private readonly MemoryStream _uncompressedBuffer;

    private bool _isDisposed;

    public ZlibStream(Stream outputStream, CompressionLevel compressionLevel)
    {
        _outputStream = outputStream ?? throw new ArgumentNullException(nameof(outputStream));
        _uncompressedBuffer = new MemoryStream();

        // Write zlib header (0x78 0x9C for default compatibility)
        _outputStream.WriteByte(0x78);
        _outputStream.WriteByte(0x9C);
        _deflateStream = new DeflateStream(outputStream, compressionLevel, true);
    }

    public override bool CanRead => false;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
        _deflateStream.Flush();
        _outputStream.Flush();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        throw new NotSupportedException("Reading is not supported.");
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException("Seeking is not supported.");
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException("Setting length is not supported.");
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        _uncompressedBuffer.Write(buffer, offset, count);
        _deflateStream.Write(buffer, offset, count);
    }

    private uint ComputeAdler32()
    {
        const uint MOD_ADLER = 65521;
        uint a = 1, b = 0;
        byte[] data = _uncompressedBuffer.ToArray();

        foreach (byte byt in data)
        {
            a = (a + byt) % MOD_ADLER;
            b = (b + a) % MOD_ADLER;
        }

        return (b << 16) | a;
    }

    protected override void Dispose(bool disposing)
    {
        if (_isDisposed)
        {
            return;
        }

        if (disposing)
        {
            _deflateStream.Dispose();

            uint adler32 = ComputeAdler32();
            byte[] checksum = BitConverter.GetBytes(adler32);
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(checksum);
            }

            _outputStream.Write(checksum, 0, checksum.Length);
            _uncompressedBuffer.Dispose();
        }

        _isDisposed = true;
        base.Dispose(disposing);
    }
}
