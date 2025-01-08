using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

internal sealed class ZipContentReader : ZipContentOpsBase
{
    internal ZipContentReader(ZipArchive zip) : base(zip) { }

    internal IEnumerable<byte> StreamBytes(ZipEntryFile entry, int bufferSize)
    {
        using Stream entryStream = entry.Open(ZipArchive);
        _buffer ??= new byte[bufferSize];
        int bytes;

        while ((bytes = entryStream.Read(_buffer, 0, bufferSize)) > 0)
        {
            for (int i = 0; i < bytes; i++)
            {
                yield return _buffer[i];
            }
        }
    }

    internal byte[] ReadAllBytes(ZipEntryFile entry)
    {
        using Stream entryStream = entry.Open(ZipArchive);
        using MemoryStream mem = new();

        entryStream.CopyTo(mem);
        return mem.ToArray();
    }

    internal IEnumerable<string> StreamLines(ZipEntryFile entry, Encoding encoding)
    {
        using Stream entryStream = entry.Open(ZipArchive);
        using StreamReader reader = new(entryStream, encoding);

        while (!reader.EndOfStream)
        {
            yield return reader.ReadLine();
        }
    }

    internal string ReadToEnd(ZipEntryFile entry, Encoding encoding)
    {
        using Stream entryStream = entry.Open(ZipArchive);
        using StreamReader reader = new(entryStream, encoding);
        return reader.ReadToEnd();
    }
}
