using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using BrotliSharpLib;
using PSCompression.Abstractions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertFrom, "BrotliString")]
[OutputType(typeof(string))]
[Alias("frombrotlistring")]
public sealed class ConvertFromBrotliStringCommand : FromCompressedStringCommandBase
{
    protected override Stream CreateDecompressionStream(Stream inputStream) =>
        new BrotliStream(inputStream, CompressionMode.Decompress);
}
