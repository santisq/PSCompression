using System.IO;
using System.Management.Automation;
using PSCompression.Abstractions;

namespace PSCompression;

internal sealed class EntryByteReader : EntryStreamOpsBase
{
    private readonly byte[] _buffer;

    private readonly int _bufferSize;

    internal EntryByteReader(Stream stream, byte[] buffer)
        : base(stream)
    {
        _buffer = buffer;
        _bufferSize = buffer.Length;
    }

    internal void StreamBytes(PSCmdlet cmdlet)
    {
        int bytes;
        while ((bytes = Stream.Read(_buffer, 0, _bufferSize)) > 0)
        {
            for (int i = 0; i < bytes; i++)
            {
                cmdlet.WriteObject(_buffer[i]);
            }
        }
    }

    internal void ReadAllBytes(PSCmdlet cmdlet)
    {
        using MemoryStream mem = new();
        Stream.CopyTo(mem);
        cmdlet.WriteObject(mem.ToArray());
    }
}
