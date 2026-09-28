using System;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using PSCompression.Extensions;

namespace PSCompression.Abstractions.Commands;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class ExpandArchiveCommandBase : PathCommandBase
{
    [Parameter(Position = 1)]
    public string? Destination { get; set; }

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing()
    {
        Destination = Destination is null
            // PowerShell is retarded and decided to mix up ProviderPath & Path
            ? SessionState.Path.CurrentFileSystemLocation.ProviderPath
            : Destination.ResolvePath(this);

        if (File.Exists(Destination))
        {
            ThrowTerminatingError(ExceptionExtensions.NotDirectoryPath(
                Destination, nameof(Destination)));
        }

        Directory.CreateDirectory(Destination);
    }

    protected override void ProcessRecord()
    {
        Dbg.Assert(Destination is not null);

        foreach (string path in EnumerateResolvedPaths())
        {
            try
            {
                PSObject[] result = ExtractArchive(path, Destination);

                if (PassThru)
                {
                    result.Sort((x, y) =>
                        string.Compare(
                            (string)x.Properties["PSParentPath"].Value,
                            (string)y.Properties["PSParentPath"].Value,
                            ignoreCase: true));

                    foreach (PSObject entry in result)
                    {
                        WriteObject(entry);
                    }
                }
            }
            catch (Exception _) when (_ is PipelineStoppedException or FlowControlException)
            {
                throw;
            }
            catch (Exception exception)
            {
                WriteError(exception.ToWriteError(path));
            }
        }
    }

    protected abstract PSObject[] ExtractArchive(string source, string destination);
}
