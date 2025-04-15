using System.IO;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertFrom, "ZLibString")]
[OutputType(typeof(string))]
[Alias("zlibfromstring")]
public sealed class ConvertFromZLibStringCommand : CommandFromCompressedStringBase
{
    protected override Stream CreateDecompressionStream(Stream inputStream)
    {
        inputStream.Seek(2, SeekOrigin.Begin);
        DeflateStream deflate = new(inputStream, CompressionMode.Decompress);
        return deflate;
    }
}
