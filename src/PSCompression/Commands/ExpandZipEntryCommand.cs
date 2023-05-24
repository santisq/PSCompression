using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsData.Expand, "ZipEntry")]
public abstract class ExpandZipEntryCommand : PSCompressionCommandsBase, IDisposable
{
    private readonly Dictionary<string, ZipArchive> _cache = new();

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryFile[] ZipEntry { get; set; } = null!;

    [Parameter(Position = 0)]
    public string? Destination { get; set; }

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing()
    {
        Destination ??= SessionState.Path.CurrentFileSystemLocation.Path;

        (string path, ProviderInfo provider) = NormalizePaths(new string[1] { Destination }, isLiteral: true)
            .FirstOrDefault();

        if (!ValidatePath(path, provider))
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
            if (!_cache.ContainsKey(entry.Source))
            {
                _cache[entry.Source] = entry.OpenZip(ZipArchiveMode.Read);
            }

            (string path, bool isfile) = entry.ExtractTo(
                _cache[entry.Source],
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
    }

    public void Dispose()
    {
        foreach (ZipArchive zip in _cache.Values)
        {
            zip?.Dispose();
        }
    }
}
