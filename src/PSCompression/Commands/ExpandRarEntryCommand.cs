using System.IO;
using System.Management.Automation;
using System.Security;
using PSCompression.Abstractions.Commands;
using PSCompression.Extensions;
using PSCompression.FormatHandlers.Common;
using PSCompression.FormatHandlers.Rar;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Common;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "RarEntry")]
[OutputType(typeof(FileInfo), typeof(DirectoryInfo))]
[Alias("unrarentry")]
public sealed class ExpandRarEntryCommand : ExpandEntryCommandBase<RarEntry>
{
    private readonly ArchiveCache<IRarArchive, RarEntry> _cache = new(e => e.OpenRead());

    private ExtractionOptions? _options;

    [Parameter]
    public SecureString? Password { get; set; }

    protected override FileSystemInfo Extract(RarEntry entry, string destination)
    {
        destination = Path.GetFullPath(Path.Combine(destination, entry.RelativePath));

        if (entry.Type == Enum.EntryType.Directory)
        {
            DirectoryInfo dir = new(destination);
            dir.Create();
            return dir;
        }

        _options ??= new ExtractionOptions(overwrite: Force);
        IRarArchive rar = _cache.GetOrCreate(entry);

        if (entry.IsEncrypted)
        {
            rar.ReaderOptions.Password = Password is null
                ? Host.PromptForPassword(entry)
                : Password.AsPlainText();
        }

        IArchiveEntry rarEntry = rar.GetEntry(entry);
        FileInfo file = new(destination);
        file.Directory?.Create();
        rarEntry.WriteToFile(destination, _options);
        return file;
    }
}
