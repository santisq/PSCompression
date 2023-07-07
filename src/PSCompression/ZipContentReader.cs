using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

internal sealed class ZipContentReader : ZipContentOpsBase
{
    internal ZipContentReader(ZipArchive zip) : base(zip)
    { }

    private Stream GetStream(string entry) =>
        ZipArchive.GetEntry(entry).Open();

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

    internal void StreamLines(string entry, Encoding encoding, PSCmdlet cmdlet)
    {
        using Stream entryStream = GetStream(entry);
        using StreamReader reader = new(entryStream, encoding);

        while (!reader.EndOfStream)
        {
            cmdlet.WriteObject(reader.ReadLine());
        }
    }

    internal string ReadToEnd(string entry, Encoding encoding)
    {
        using Stream entryStream = GetStream(entry);
        using StreamReader reader = new(entryStream, encoding);

        return reader.ReadToEnd();
    }
}
