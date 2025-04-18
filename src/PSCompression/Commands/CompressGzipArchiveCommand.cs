using System.IO;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Compress, "GzipArchive", DefaultParameterSetName = "Path")]
[OutputType(typeof(FileInfo))]
[Alias("togzipfile")]
public sealed class CompressGzipArchive : CommandToCompressedFileBase
{
    protected override string FileExtension => ".gz";

    protected override Stream CreateCompressionStream(
        Stream destinationStream,
        CompressionLevel compressionLevel)
    {
        return new GZipStream(destinationStream, compressionLevel);
    }
}
