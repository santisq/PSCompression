using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsData.Expand, "ZipEntry")]
[OutputType(typeof(FileSystemInfo), ParameterSetName = new string[1] { "PassThru" })]
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

    [Parameter(ParameterSetName = "PassThru")]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing()
    {
        Destination ??= SessionState.Path.CurrentFileSystemLocation.Path;

        try
        {
            (string? path, ProviderInfo? provider) = Destination
                .NormalizePath(isLiteral: true, this);

            if (path is null || provider is null)
            {
                return;
            }

            if (!provider.AssertFileSystem())
            {
                ThrowTerminatingError(
                    ExceptionHelpers.NotFileSystemPathError(Destination, provider));
            }

            if (!path.AssertDirectory())
            {
                ThrowTerminatingError(
                    ExceptionHelpers.NotDirectoryPathError(path));
            }

            Destination = path;
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(
                ExceptionHelpers.ResolvePathError(Destination, e));
        }
    }

    protected override void ProcessRecord()
    {
        if (Destination is null)
        {
            return;
        }

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
                WriteError(ExceptionHelpers.ExtractEntryError(entry, e));
            }
        }
    }

    public void Dispose() => _cache?.Dispose();
}
