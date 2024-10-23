using System;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Exceptions;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.Remove, "ZipEntry", SupportsShouldProcess = true)]
public sealed class RemoveZipEntryCommand : PSCmdlet, IDisposable
{
    private readonly ZipArchiveCache _cache = new(ZipArchiveMode.Update);

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryBase[] InputObject { get; set; } = null!;

    protected override void ProcessRecord()
    {
        foreach (ZipEntryBase entry in InputObject)
        {
            try
            {
                if (ShouldProcess(target: entry.ToString(), action: "Remove"))
                {
                    entry.Remove(_cache.GetOrAdd(entry));
                }
            }
            catch (Exception exception)
            {
                WriteError(exception.ToOpenError(entry.Source));
            }
        }
    }

    public void Dispose() => _cache?.Dispose();
}
