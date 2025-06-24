using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Abstractions;
using PSCompression.Extensions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertTo, "BrotliString")]
[OutputType(typeof(byte[]), typeof(string))]
[Alias("tobrotlistring")]
public sealed class ConvertToBrotliStringCommand : ToCompressedStringCommandBase
{
    protected override Stream CreateCompressionStream(
        Stream outputStream,
        CompressionLevel compressionLevel)
        => outputStream.AsBrotliCompressedStream(compressionLevel);
}
