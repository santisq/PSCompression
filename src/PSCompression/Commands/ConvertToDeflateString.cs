using System.IO;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertTo, "DeflateString")]
[OutputType(typeof(string))]
[Alias("deflatetostring")]
public sealed class ConvertToDeflateString : CommandToCompressedStringBase
{
    protected override Stream CreateCompressionStream(
        Stream outputStream,
        CompressionLevel compressionLevel)
    {
        return new DeflateStream(outputStream, compressionLevel);
    }
}
