using System.Management.Automation;
using PSCompression.Abstractions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "TarArchive")]
public sealed class ExpandTarArchiveCommand : CommandWithPathBase
{
    private bool _shouldInferAlgo;

    [Parameter(Mandatory = true, Position = 1)]
    public string Destination { get; set; } = null!;

    [Parameter]
    public Algorithm Algorithm { get; set; }

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing() =>
        _shouldInferAlgo = !MyInvocation.BoundParameters.ContainsKey(nameof(Algorithm));

    protected override void ProcessRecord()
    {
        foreach (string path in EnumerateResolvedPaths())
        {
            if (_shouldInferAlgo)
            {
                Algorithm = AlgorithmMappings.Parse(path);
            }
        }
    }
}
