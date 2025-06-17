using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Abstractions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertFrom, "GzipString")]
[OutputType(typeof(string))]
[Alias("fromgzipstring")]
public sealed class ConvertFromGzipStringCommand : FromCompressedStringCommandBase
{
    protected override Stream CreateDecompressionStream(Stream inputStream) =>
        new GZipStream(inputStream, CompressionMode.Decompress);
}
