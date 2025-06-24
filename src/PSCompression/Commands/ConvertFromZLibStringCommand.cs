using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Abstractions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertFrom, "ZLibString")]
[OutputType(typeof(string))]
[Alias("fromzlibstring")]
public sealed class ConvertFromZLibStringCommand : FromCompressedStringCommandBase
{
    protected override Stream CreateDecompressionStream(Stream inputStream)
    {
        inputStream.Seek(2, SeekOrigin.Begin);
        DeflateStream deflate = new(inputStream, CompressionMode.Decompress);
        return deflate;
    }
}
