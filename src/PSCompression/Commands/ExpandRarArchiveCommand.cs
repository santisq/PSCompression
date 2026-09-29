using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Security;
using PSCompression.Abstractions.Commands;
using PSCompression.Extensions;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;
using IOPath = System.IO.Path;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "RarArchive")]
[OutputType(typeof(FileInfo), typeof(DirectoryInfo))]
[Alias("unrar")]
public sealed class ExpandRarArchiveCommand : ExpandArchiveCommandBase
{
    [Parameter]
    public SecureString? Password { get; set; }

    protected override List<PSObject> ExtractArchive(string source, string destination)
    {
        IRarArchive rar;
        using (rar = RarArchive.OpenArchive(source))
        {
            if (rar.IsEncrypted)
            {
                rar.ReaderOptions.Password = Password is null
                    ? Host.PromptForPassword($"Encrypted RAR Archive '{source}' requires a password.")
                    : Password.AsPlainText();
            }
        }

        using (rar = RarArchive.OpenArchive(source, rar.ReaderOptions))
            rar.WriteToDirectory(destination, new ExtractionOptions(overwrite: Force));

        return !PassThru ? [] : [.. rar.Entries.Select(e =>
        {
            string dest = IOPath.GetFullPath(IOPath.Combine(destination, e.Key ?? ""));
            FileSystemInfo info = e.IsDirectory ? new DirectoryInfo(dest) : new FileInfo(dest);
            return info.AppendPSProperties();
        })];
    }
}
