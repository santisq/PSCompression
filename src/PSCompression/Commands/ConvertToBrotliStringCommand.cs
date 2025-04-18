using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using Brotli;
using PSCompression.Exceptions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.ConvertTo, "BrotliString")]
[OutputType(typeof(byte[]), typeof(string))]
[Alias("tobrotlistring")]
public sealed class ConvertToBrotliStringCommand : CommandToCompressedStringBase
{
    [Parameter(DontShow = true)]
    public override CompressionLevel CompressionLevel { get; set; }

    protected override void BeginProcessing()
    {
        this.WriteWarningForIgnoredParameter(
            MyInvocation.BoundParameters.ContainsKey(nameof(CompressionLevel)),
            nameof(CompressionLevel),
            "Brotli.NET");

        base.BeginProcessing();
    }

    protected override Stream CreateCompressionStream(
        Stream outputStream,
        CompressionLevel compressionLevel)
    {
        return new BrotliStream(outputStream, CompressionMode.Compress);
    }
}
