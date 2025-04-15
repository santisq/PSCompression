using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using Brotli;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertTo, "BrotliString")]
[OutputType(typeof(string))]
[Alias("tobrotlistring")]
public sealed class ConvertToBrotliStringCommand : CommandToCompressedStringBase
{
    protected override Stream CreateCompressionStream(
        Stream outputStream,
        CompressionLevel compressionLevel)
    {
        return new BrotliStream(outputStream, CompressionMode.Compress);
    }
}
