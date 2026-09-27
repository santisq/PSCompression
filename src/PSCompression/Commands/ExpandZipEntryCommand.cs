using System;
using System.IO;
using System.Management.Automation;
using System.Security;
using ICSharpCode.SharpZipLib.Zip;
using PSCompression.Abstractions;
using PSCompression.Extensions;
using PSCompression.FormatHandlers.Common;
using PSCompression.FormatHandlers.Zip;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "ZipEntry")]
[OutputType(typeof(FileInfo), typeof(DirectoryInfo))]
[Alias("unzipentry")]
public sealed class ExpandZipEntryCommand : ExpandEntryCommandBase<ZipEntryBase>, IDisposable
{
    private ArchiveCache<ZipFile, ZipEntryBase>? _cache;

    [Parameter]
    public SecureString? Password { get; set; }

    protected override FileSystemInfo Extract(ZipEntryBase entry, string destination)
    {
        _cache ??= new(entry => entry.OpenRead(Password));
        ZipFile zip = _cache.GetOrCreate(entry);

        if (entry.IsEncrypted && Password is null && entry is ZipEntryFile fileEntry)
            zip.Password = fileEntry.PromptForPassword(Host);

        return entry.ExtractTo(destination, Force, zip);
    }

    public void Dispose() => _cache?.Dispose();
}
