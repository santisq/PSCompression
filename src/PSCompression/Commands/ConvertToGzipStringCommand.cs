using System.IO;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertTo, "GzipString")]
[OutputType(typeof(byte[]), typeof(string))]
[Alias("togzipstring")]
public sealed class ConvertToGzipStringCommand : CommandToCompressedStringBase
{
    protected override Stream CreateCompressionStream(
        Stream outputStream,
        CompressionLevel compressionLevel)
    {
        return new GZipStream(outputStream, compressionLevel);
    }
}
