using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

public class ZipContentReader : IDisposable
{
    public string Source { get; }

    public ZipArchive ZipArchive { get; }

    private byte[]? _buffer;

    private readonly List<string> _content = new();

    internal ZipContentReader(string source)
    {
        ZipArchive = ZipFile.OpenRead(source);
        Source = source;
    }

    private Stream GetStream(string entry) =>
        ZipArchive.GetEntry(entry).Open();

    internal void StreamLines(string entry, Encoding encoding, PSCmdlet cmdlet)
    {
        using Stream entryStream = GetStream(entry);
        using StreamReader reader = new(entry, encoding);

        while (!reader.EndOfStream)
        {
            cmdlet.WriteObject(reader.ReadLine());
        }
    }

    internal void StreamBytes(string entry, int bufferSize, PSCmdlet cmdlet)
    {
        using Stream entryStream = GetStream(entry);

        int bytes;
        _buffer ??= new byte[bufferSize];

        while ((bytes = entryStream.Read(_buffer, 0, bufferSize)) > 0)
        {
            cmdlet.WriteObject(_buffer.Take(bytes), enumerateCollection: true);
        }
    }

    internal byte[] ReadAllBytes(string entry)
    {
        using Stream entryStream = GetStream(entry);
        using MemoryStream mem = new();

        entryStream.CopyTo(mem);
        return mem.ToArray();
    }

    internal string ReadToEnd(string entry, Encoding encoding)
    {
        using Stream entryStream = GetStream(entry);
        using StreamReader reader = new(entryStream, encoding);

        return reader.ReadToEnd();
    }

    internal string[] ReadAllLines(string entry, Encoding encoding)
    {
        using Stream entryStream = GetStream(entry);
        using StreamReader reader = new(entryStream, encoding);

        _content.Clear();

        while (!reader.EndOfStream)
        {
            _content.Add(reader.ReadLine());
        }

        return _content.ToArray();
    }

    public void Dispose() => ZipArchive?.Dispose();
}
