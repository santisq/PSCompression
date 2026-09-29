using System;
using System.IO;
using PSCompression.Abstractions.Entries;

namespace PSCompression.FormatHandlers.Zip;

internal sealed class ZipEntryByteWriter : EntryStreamOpsBase
{
    private readonly byte[] _buffer;

    private readonly int _bufferSize;

    private int _index;

    internal ZipEntryByteWriter(Stream stream, int bufferSize, bool append = false)
        : base(stream)
    {
        _buffer = new byte[bufferSize];
        _bufferSize = bufferSize;

        if (append)
        {
            Stream.Seek(0, SeekOrigin.End);
            return;
        }

        Stream.SetLength(0);
    }

    internal void WriteBytes(byte[] bytes)
    {
        int offset = 0;
        int remaining = bytes.Length;

        while (remaining > 0)
        {
            if (_index == _bufferSize)
            {
                Stream.Write(_buffer, 0, _index);
                _index = 0;
            }

            int toCopy = Math.Min(remaining, _bufferSize - _index);
            Array.Copy(bytes, offset, _buffer, _index, toCopy);

            _index += toCopy;
            offset += toCopy;
            remaining -= toCopy;
        }
    }

    private void Flush()
    {
        if (_index > 0)
        {
            Stream.Write(_buffer, 0, _index);
            Stream.Flush();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            Flush();
        }

        base.Dispose(disposing);
    }
}
