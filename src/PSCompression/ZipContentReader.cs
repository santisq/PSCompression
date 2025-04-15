using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;
using PSCompression.Extensions;

namespace PSCompression;

internal sealed class ZipContentReader : ZipContentOpsBase
{
    private byte[]? _buffer;

    internal ZipContentReader(ZipArchive zip) : base(zip)
    { }

    internal void StreamBytes(
        ZipEntryFile entry,
        int bufferSize,
        PSCmdlet cmdlet)
    {
        int bytes;
        using Stream entryStream = entry.Open(_zip);
        _buffer ??= new byte[bufferSize];

        while ((bytes = entryStream.Read(_buffer, 0, bufferSize)) > 0)
        {
            for (int i = 0; i < bytes; i++)
            {
                cmdlet.WriteObject(_buffer[i]);
            }
        }
    }

    internal void ReadAllBytes(ZipEntryFile entry, PSCmdlet cmdlet)
    {
        using Stream entryStream = entry.Open(_zip);
        using MemoryStream mem = new();

        entryStream.CopyTo(mem);
        cmdlet.WriteObject(mem.ToArray());
    }

    internal void StreamLines(
        ZipEntryFile entry,
        Encoding encoding,
        PSCmdlet cmdlet)
    {
        using Stream entryStream = entry.Open(_zip);
        using StreamReader reader = new(entryStream, encoding);
        reader.ReadLines(cmdlet);
    }

    internal void ReadToEnd(
        ZipEntryFile entry,
        Encoding encoding,
        PSCmdlet cmdlet)
    {
        using Stream entryStream = entry.Open(_zip);
        using StreamReader reader = new(entryStream, encoding);
        reader.ReadToEnd(cmdlet);
    }
}
