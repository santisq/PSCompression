using System.IO;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertTo, "ZLibString")]
[OutputType(typeof(byte[]), typeof(string))]
[Alias("tozlibstring")]
public sealed class ConvertToZLibStringCommand : CommandToCompressedStringBase
{
    protected override Stream CreateCompressionStream(
        Stream outputStream,
        CompressionLevel compressionLevel)
    {
        return new ZlibStream(outputStream, compressionLevel);
    }
}
