using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Abstractions.Commands;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertTo, "GzipString")]
[OutputType(typeof(byte[]), typeof(string))]
[Alias("togzipstring")]
public sealed class ConvertToGzipStringCommand : ToCompressedStringCommandBase
{
    protected override Stream CreateCompressionStream(
        Stream outputStream,
        CompressionLevel compressionLevel)
        => new GZipStream(outputStream, compressionLevel);
}
