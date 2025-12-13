using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Abstractions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "ZipEntry")]
[OutputType(typeof(FileInfo), typeof(DirectoryInfo))]
[Alias("unzipentry")]
public sealed class ExpandZipEntryCommand : ExpandEntryCommandBase<ZipEntryBase>, IDisposable
{
    private readonly ZipArchiveCache<ZipArchive> _cache = new(entry => entry.OpenRead());

    protected override FileSystemInfo Extract(ZipEntryBase entry) =>
        entry.ExtractTo(Destination!, Force, _cache.GetOrCreate(entry));

    public void Dispose()
    {
        _cache?.Dispose();
        GC.SuppressFinalize(this);
    }
}
