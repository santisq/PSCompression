using System;
using System.IO;
using System.Management.Automation;
using PSCompression.Extensions;
using PSCompression.Exceptions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "ZipEntry")]
[OutputType(typeof(FileSystemInfo))]
public sealed class ExpandZipEntryCommand : PSCmdlet, IDisposable
{
    private readonly ZipArchiveCache _cache = new();

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryBase[] InputObject { get; set; } = null!;

    [Parameter(Position = 0)]
    [ValidateNotNullOrEmpty]
    public string? Destination { get; set; }

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing()
    {
        Destination = Destination is null
            ? SessionState.Path.CurrentFileSystemLocation.Path
            : Destination.ResolvePath(this);

        if (Destination.IsArchive())
        {
            ThrowTerminatingError(
                ExceptionHelper.NotDirectoryPath(
                    Destination,
                    nameof(Destination)));
        }

        if (!Directory.Exists(Destination))
        {
            Directory.CreateDirectory(Destination);
        }
    }

    protected override void ProcessRecord()
    {
        Dbg.Assert(Destination is not null);

        foreach (ZipEntryBase entry in InputObject)
        {
            try
            {
                (string path, bool isfile) = entry.ExtractTo(
                    _cache.GetOrAdd(entry),
                    Destination,
                    Force.IsPresent);

                if (PassThru.IsPresent)
                {
                    if (isfile)
                    {
                        WriteObject(new FileInfo(path));
                        continue;
                    }

                    WriteObject(new DirectoryInfo(path));
                }
            }
            catch (Exception _) when (_ is PipelineStoppedException or FlowControlException)
            {
                throw;
            }
            catch (Exception exception)
            {
                WriteError(exception.ToExtractEntryError(entry));
            }
        }
    }

    public void Dispose()
    {
        _cache?.Dispose();
        GC.SuppressFinalize(this);
    }
}
