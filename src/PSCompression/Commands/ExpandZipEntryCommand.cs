using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsData.Expand, "ZipEntry")]
[OutputType(typeof(FileSystemInfo), ParameterSetName = new string[1] { "PassThru" })]
public sealed class ExpandZipEntryCommand : PSCompressionCommandsBase, IDisposable
{
    private readonly ZipArchiveCache _cache = new();

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryBase[] ZipEntry { get; set; } = null!;

    [Parameter(Position = 0)]
    public string? Destination { get; set; }

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter(ParameterSetName = "PassThru")]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing()
    {
        Destination ??= SessionState.Path.CurrentFileSystemLocation.Path;

        (string path, ProviderInfo provider) = NormalizePaths(
            new string[1] { Destination }, isLiteral: true)
            .FirstOrDefault();

        if (!ValidatePath(path, provider, assertFile: false))
        {
            Destination = null;
            return;
        }

        if (!Directory.Exists(path))
        {
            Destination = null;
            ThrowTerminatingError(new ErrorRecord(
                new ArgumentException($"Destination must be an existing directory: '{path}'."),
                "PathIsDirectory", ErrorCategory.InvalidArgument, path));
        }
    }

    protected override void ProcessRecord()
    {
        if (Destination is null)
        {
            return;
        }

        foreach (ZipEntryBase entry in ZipEntry)
        {
            try
            {
                (string path, bool isfile) = entry.ExtractTo(
                    _cache.GetOrAdd(entry, ZipArchiveMode.Read),
                    Destination,
                    Force.IsPresent);

                if (PassThru.IsPresent)
                {
                    if (isfile)
                    {
                        WriteObject(new FileInfo(path));
                        return;
                    }

                    WriteObject(new DirectoryInfo(path));
                }
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(
                    e, "ExtractEntry", ErrorCategory.NotSpecified, entry));
            }
        }
    }

    public void Dispose() => _cache?.Dispose();
}
