using System.IO.Compression;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

[Cmdlet(VerbsData.ConvertTo, "GzipString")]
[OutputType(typeof(byte), ParameterSetName = new string[1] { "ByteSteream" })]
[OutputType(typeof(string))]
[Alias("gziptostring")]
public sealed class ConvertToGzipStringCommand : PSCmdlet
{
    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    [AllowEmptyString]
    public string[] InputObject { get; set; } = null!;

    [ArgumentCompleter(typeof(EncodingCompleter))]
    [EncodingTransformation]
    [ValidateNotNullOrEmpty]
    public Encoding Encoding { get; set; } = new UTF8Encoding();

    [Parameter]
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    [Parameter(ParameterSetName = "ByteStream")]
    [Alias("Raw")]
    public SwitchParameter AsByteStream { get; set; }

    [Parameter]
    public SwitchParameter NoNewLine { get; set; }
}
