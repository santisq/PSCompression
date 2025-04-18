using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using Brotli;
using PSCompression.Extensions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertTo, "BrotliString")]
[OutputType(typeof(byte[]), typeof(string))]
[Alias("tobrotlistring")]
public sealed class ConvertToBrotliStringCommand : CommandToCompressedStringBase
{
    protected override Stream CreateCompressionStream(
        Stream outputStream,
        CompressionLevel compressionLevel)
    {
        return outputStream.AsBrotliCompressedStream(compressionLevel);
    }
}
