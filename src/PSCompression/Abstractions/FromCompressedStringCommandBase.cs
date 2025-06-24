using System;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Text;
using PSCompression.Exceptions;
using PSCompression.Extensions;

namespace PSCompression.Abstractions;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class FromCompressedStringCommandBase : PSCmdlet
{
    protected delegate Stream DecompressionStreamFactory(Stream inputStream);

    [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
    public string[] InputObject { get; set; } = null!;

    [Parameter]
    [ArgumentCompleter(typeof(EncodingCompleter))]
    [EncodingTransformation]
    [ValidateNotNullOrEmpty]
    public Encoding Encoding { get; set; } = new UTF8Encoding();

    [Parameter]
    public SwitchParameter Raw { get; set; }

    private void Decompress(
        string base64string,
        DecompressionStreamFactory decompressionStreamFactory)
    {
        using MemoryStream inStream = new(Convert.FromBase64String(base64string));
        using Stream decompressStream = decompressionStreamFactory(inStream);
        using StreamReader reader = new(decompressStream, Encoding);

        if (Raw)
        {
            reader.WriteAllTextToPipeline(this);
            return;
        }

        reader.WriteLinesToPipeline(this);
    }

    protected abstract Stream CreateDecompressionStream(Stream inputStream);

    protected override void ProcessRecord()
    {
        foreach (string line in InputObject)
        {
            try
            {
                Decompress(line, CreateDecompressionStream);
            }
            catch (Exception _) when (_ is PipelineStoppedException or FlowControlException)
            {
                throw;
            }
            catch (Exception exception)
            {
                WriteError(exception.ToEnumerationError(line));
            }
        }
    }
}
