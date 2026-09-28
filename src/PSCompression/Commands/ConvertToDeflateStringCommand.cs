using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Abstractions.Commands;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertTo, "DeflateString")]
[OutputType(typeof(byte[]), typeof(string))]
[Alias("todeflatestring")]
public sealed class ConvertToDeflateStringCommand : ToCompressedStringCommandBase
{
    protected override Stream CreateCompressionStream(
        Stream outputStream,
        CompressionLevel compressionLevel)
        => new DeflateStream(outputStream, compressionLevel);
}
