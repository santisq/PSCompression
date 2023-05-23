using System.IO;
using System.IO.Compression;
using System.Text;

namespace PSCompression;

internal sealed class ZipContentWriter : ZipContentOpsBase
{
    private int _index;

    protected override ZipArchive ZipArchive { get => _zipEntryStream.ZipStream; }

    private readonly StreamWriter? _writer;

    private readonly ZipEntryStream _zipEntryStream;

    internal ZipContentWriter(ZipEntryFile entry, bool append, int bufferSize) :
        base(entry.Source)
    {
        _zipEntryStream = entry.OpenWrite();
        _buffer = new byte[bufferSize];

        if (append)
        {
            _zipEntryStream.Seek(0, SeekOrigin.End);
            return;
        }

        _zipEntryStream.SetLength(0);
    }

    internal ZipContentWriter(ZipEntryFile entry, bool append, Encoding encoding) :
        base(entry.Source)
    {
        _zipEntryStream = entry.OpenWrite();
        _writer = new(_zipEntryStream, encoding);

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
                _zipEntryStream.Write(_buffer, 0, _index);
                _index = 0;
            }

            _buffer[_index++] = b;
        }
    }

    public void Flush()
    {
        if(_index > 0 && _buffer is not null)
        {
            _zipEntryStream.Write(_buffer, 0, _index);
        }
    }

    protected override void Dispose(bool disposing)
    {
        Flush();
        base.Dispose(disposing);

        if (!_disposed)
        {
            _writer?.Dispose();
            _disposed = true;
        }
    }
}
