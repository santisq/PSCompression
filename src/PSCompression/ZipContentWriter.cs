using System.IO;
using System.IO.Compression;
using System.Text;

namespace PSCompression;

internal sealed class ZipContentWriter : ZipContentOpsBase
{
    private int _index;

    private readonly StreamWriter? _writer;

    private readonly Stream _stream;

    internal ZipContentWriter(ZipEntryFile entry, bool append, int bufferSize)
        : base(entry.OpenWrite())
    {
        _stream = entry.Open(_zip);
        _buffer = new byte[bufferSize];

        if (append)
        {
            _stream.Seek(0, SeekOrigin.End);
            return;
        }

        _stream.SetLength(0);
    }

    internal ZipContentWriter(ZipArchive zip, ZipArchiveEntry entry, Encoding encoding)
        : base(zip)
    {
        _stream = entry.Open();
        _writer = new StreamWriter(_stream, encoding);
    }

    internal ZipContentWriter(ZipEntryFile entry, bool append, Encoding encoding)
        : base(entry.OpenWrite())
    {
        _stream = entry.Open(_zip);
        _writer = new StreamWriter(_stream, encoding);

        if (append)
        {
            _writer.BaseStream.Seek(0, SeekOrigin.End);
            return;
        }

        _writer.BaseStream.SetLength(0);
    }

    internal void WriteLines(string[] lines)
    {
        if (_writer is null)
        {
            return;
        }

        foreach (string line in lines)
        {
            _writer.WriteLine(line);
        }
    }

    internal void WriteBytes(byte[] bytes)
    {
        if (_buffer is null)
        {
            return;
        }

        foreach (byte b in bytes)
        {
            if (_index == _buffer.Length)
            {
                _stream.Write(_buffer, 0, _index);
                _index = 0;
            }

            _buffer[_index++] = b;
        }
    }

    public void Flush()
    {
        if (_index > 0 && _buffer is not null)
        {
            _stream.Write(_buffer, 0, _index);
            _index = 0;
            _stream.Flush();
        }

        if (_writer is { BaseStream.CanWrite: true })
        {
            _writer.Flush();
        }
    }

    public void Close()
    {
        if (_writer is not null)
        {
            _writer.Close();
            return;
        }

        _stream.Close();
    }

    protected override void Dispose(bool disposing)
    {
        try
        {
            if (disposing && !_disposed)
            {
                Flush();
            }
        }
        finally
        {
            _writer?.Dispose();
            _stream.Dispose();
            base.Dispose(disposing);
        }
    }
}
