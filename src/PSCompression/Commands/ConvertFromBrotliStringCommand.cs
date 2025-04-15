using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using Brotli;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertFrom, "BrotliString")]
[OutputType(typeof(string))]
[Alias("brotlifromstring")]
public sealed class ConvertFromBrotliStringCommand : CommandFromCompressedStringBase
{
    protected override Stream CreateDecompressionStream(Stream inputStream) =>
        new BrotliStream(inputStream, CompressionMode.Decompress);
}
