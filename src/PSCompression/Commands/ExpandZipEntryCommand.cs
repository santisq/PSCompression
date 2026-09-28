using System;
using System.IO;
using System.Management.Automation;
using System.Security;
using ICSharpCode.SharpZipLib.Zip;
using PSCompression.Abstractions.Commands;
using PSCompression.Abstractions.Entries;
using PSCompression.Extensions;
using PSCompression.FormatHandlers.Common;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "ZipEntry")]
[OutputType(typeof(FileInfo), typeof(DirectoryInfo))]
[Alias("unzipentry")]
public sealed class ExpandZipEntryCommand : ExpandEntryCommandBase<ZipEntryBase>, IDisposable
{
    private readonly ArchiveCache<ZipFile, ZipEntryBase> _cache = new(entry => entry.OpenSharpZipLibArchive());

    [Parameter]
    public SecureString? Password { get; set; }

    protected override FileSystemInfo Extract(ZipEntryBase entry, string destination)
    {
        ZipFile zip = _cache.GetOrCreate(entry);

        if (entry.IsEncrypted)
        {
            zip.Password = Password is null
                ? Host.PromptForPassword(entry)
                : Password.AsPlainText();
        }

        return entry.ExtractTo(destination, Force, zip);
    }

    public void Dispose() => _cache?.Dispose();
}
