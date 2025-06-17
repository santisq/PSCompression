using System;
using System.IO;
using System.Management.Automation;
using PSCompression.Abstractions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "ZipEntry")]
[OutputType(typeof(FileSystemInfo))]
public sealed class ExpandZipEntryCommand : ExpandEntryCommandBase<ZipEntryBase>, IDisposable
{
    private readonly ZipArchiveCache _cache = new();

    protected override FileSystemInfo Extract(ZipEntryBase entry) =>
        entry.ExtractTo(Destination!, Force, _cache.GetOrAdd(entry));

    public void Dispose()
    {
        _cache?.Dispose();
        GC.SuppressFinalize(this);
    }
}
