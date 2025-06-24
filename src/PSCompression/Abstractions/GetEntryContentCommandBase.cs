using System.ComponentModel;
using System.Management.Automation;
using System.Text;

namespace PSCompression.Abstractions;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class GetEntryContentCommandBase<T> : PSCmdlet
    where T : EntryBase
{
    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public T[] Entry { get; set; } = null!;

    [Parameter(ParameterSetName = "Stream")]
    [ArgumentCompleter(typeof(EncodingCompleter))]
    [EncodingTransformation]
    [ValidateNotNullOrEmpty]
    public Encoding Encoding { get; set; } = new UTF8Encoding();

    [Parameter]
    public SwitchParameter Raw { get; set; }

    [Parameter(ParameterSetName = "Bytes")]
    public SwitchParameter AsByteStream { get; set; }

    [Parameter(ParameterSetName = "Bytes")]
    [ValidateNotNullOrEmpty]
    public int BufferSize { get; set; } = 128_000;
}
