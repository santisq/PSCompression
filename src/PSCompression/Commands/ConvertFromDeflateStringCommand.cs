using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Abstractions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertFrom, "DeflateString")]
[OutputType(typeof(string))]
[Alias("fromdeflatestring")]
public sealed class ConvertFromDeflateStringCommand : FromCompressedStringCommandBase
{
    protected override Stream CreateDecompressionStream(Stream inputStream) =>
        new DeflateStream(inputStream, CompressionMode.Decompress);
}
