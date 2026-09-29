using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Abstractions.Commands;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertFrom, "ZLibString")]
[OutputType(typeof(string))]
[Alias("fromzlibstring")]
public sealed class ConvertFromZLibStringCommand : FromCompressedStringCommandBase
{
    protected override Stream CreateDecompressionStream(Stream inputStream)
    {
        inputStream.Position = 2;
        DeflateStream deflate = new(inputStream, CompressionMode.Decompress);
        return deflate;
    }
}
