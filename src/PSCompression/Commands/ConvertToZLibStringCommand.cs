using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Abstractions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertTo, "ZLibString")]
[OutputType(typeof(byte[]), typeof(string))]
[Alias("tozlibstring")]
public sealed class ConvertToZLibStringCommand : ToCompressedStringCommandBase
{
    protected override Stream CreateCompressionStream(
        Stream outputStream,
        CompressionLevel compressionLevel)
        => new ZlibStream(outputStream, compressionLevel);
}
