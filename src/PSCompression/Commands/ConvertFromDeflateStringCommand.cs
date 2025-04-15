using System.IO;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertFrom, "DeflateString")]
[OutputType(typeof(string))]
[Alias("fromdeflatestring")]
public sealed class ConvertFromDeflateStringCommand : CommandFromCompressedStringBase
{
    protected override Stream CreateDecompressionStream(Stream inputStream) =>
        new DeflateStream(inputStream, CompressionMode.Decompress);
}
