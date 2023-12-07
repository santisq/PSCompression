using System.IO;
using System.IO.Compression;
using System.Text;

namespace PSCompression;

internal sealed class ZipContentWriter : ZipContentOpsBase
{
    private int _index;

    public override ZipArchive ZipArchive { get; }

    private readonly StreamWriter? _writer;

    private readonly Stream Stream;

    private bool _disposed;

    internal ZipContentWriter(ZipEntryFile entry, bool append, int bufferSize)
    {
        ZipArchive = entry.OpenWrite();
        Stream = ZipArchive.GetEntry(entry.RelativePath).Open();
        _buffer = new byte[bufferSize];

        if (append)
        {
            Stream.Seek(0, SeekOrigin.End);
            return;
        }

        Stream.SetLength(0);
    }

    internal ZipContentWriter(ZipArchive zip, ZipArchiveEntry entry, Encoding encoding)
    {
        ZipArchive = zip;
        Stream = entry.Open();
        _writer = new StreamWriter(Stream, encoding);
    }

    internal ZipContentWriter(ZipEntryFile entry, bool append, Encoding encoding)
    {
        ZipArchive = entry.OpenWrite();
        Stream = ZipArchive.GetEntry(entry.RelativePath).Open();
        _writer = new StreamWriter(Stream, encoding);

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
                Stream.Write(_buffer, 0, _index);
                _index = 0;
            }

            _buffer[_index++] = b;
        }
    }

    public void Flush()
    {
        if (_index > 0 && _buffer is not null)
        {
            Stream.Write(_buffer, 0, _index);
            _index = 0;
            Stream.Flush();
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

        Stream.Close();
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
            Stream.Dispose();
            _disposed = true;
            base.Dispose(disposing);
        }
    }
}
