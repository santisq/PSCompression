using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using Brotli;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertTo, "BrotliString")]
[OutputType(typeof(byte[]), typeof(string))]
[Alias("tobrotlistring")]
public sealed class ConvertToBrotliStringCommand : CommandToCompressedStringBase
{
    private uint MapCompressionLevelToBrotliQuality(CompressionLevel compressionLevel) =>
        compressionLevel switch
        {
            CompressionLevel.NoCompression => 0,
            CompressionLevel.Fastest => 1,
            _ => 11
        };

    protected override Stream CreateCompressionStream(
        Stream outputStream,
        CompressionLevel compressionLevel)
    {
        BrotliStream brotli = new(outputStream, CompressionMode.Compress);
        brotli.SetQuality(MapCompressionLevelToBrotliQuality(compressionLevel));
        return brotli;
    }
}
