using System.IO;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertFrom, "GzipString")]
[OutputType(typeof(string))]
[Alias("fromgzipstring")]
public sealed class ConvertFromGzipStringCommand : CommandFromCompressedStringBase
{
    protected override Stream CreateDecompressionStream(Stream inputStream) =>
        new GZipStream(inputStream, CompressionMode.Decompress);
}
