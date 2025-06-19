using System;
using System.IO;
using System.Management.Automation;
using PSCompression.Extensions;
using PSCompression.Exceptions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PSCompression.Abstractions;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class ExpandEntryCommandBase<T> : PSCmdlet
    where T : EntryBase
{
    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public T[] InputObject { get; set; } = null!;

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

        if (File.Exists(Destination))
        {
            ThrowTerminatingError(ExceptionHelper.NotDirectoryPath(
                Destination, nameof(Destination)));
        }

        Directory.CreateDirectory(Destination);
    }

    protected override void ProcessRecord()
    {
        Dbg.Assert(Destination is not null);

        foreach (T entry in InputObject)
        {
            try
            {
                FileSystemInfo info = Extract(entry);

                if (PassThru)
                {
                    string parent = info is DirectoryInfo dir
                        ? dir.Parent.FullName
                        : Unsafe.As<FileInfo>(info).DirectoryName;

                    WriteObject(info.AppendPSProperties(parent));
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

    protected abstract FileSystemInfo Extract(T entry);
}
