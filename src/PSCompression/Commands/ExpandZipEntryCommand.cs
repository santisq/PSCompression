using System;
using System.IO;
using System.Management.Automation;
using System.Security;
using ICSharpCode.SharpZipLib.Zip;
using PSCompression.Abstractions;
using PSCompression.Extensions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "ZipEntry")]
[OutputType(typeof(FileInfo), typeof(DirectoryInfo))]
[Alias("unzipentry")]
public sealed class ExpandZipEntryCommand : ExpandEntryCommandBase<ZipEntryBase>, IDisposable
{
    [Parameter]
    public SecureString? Password { get; set; }

    private ZipArchiveCache<ZipFile>? _cache;

    protected override FileSystemInfo Extract(ZipEntryBase entry)
    {
        _cache ??= new ZipArchiveCache<ZipFile>(entry => entry.OpenRead(Password));
        ZipFile zip = _cache.GetOrCreate(entry);

        if (entry.IsEncrypted && Password is null && entry is ZipEntryFile fileEntry)
        {
            zip.Password = fileEntry.PromptForPassword(Host);
        }

        return entry.ExtractTo(Destination!, Force, zip);
    }

    public void Dispose()
    {
        _cache?.Dispose();
        GC.SuppressFinalize(this);
    }
}
