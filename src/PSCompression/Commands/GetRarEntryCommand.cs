using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using PSCompression.Abstractions.Commands;
using PSCompression.Abstractions.Entries;
using PSCompression.FormatHandlers.Rar;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.Get, "RarEntry", DefaultParameterSetName = "Path")]
[OutputType(typeof(RarEntry))]
[Alias("rarge")]
public sealed class ExpandRarArchive : GetEntryCommandBase
{
    internal override Enum.ArchiveType ArchiveType => Enum.ArchiveType.rar;

    protected override IEnumerable<EntryBase> GetEntriesFromFile(string path)
    {
        using IRarArchive rar = RarArchive.OpenArchive(path);
        foreach (IArchiveEntry entry in rar.Entries)
        {
            if (ShouldSkipEntry(entry.IsDirectory))
                continue;

            if (!ShouldInclude(entry.Key) || ShouldExclude(entry.Key))
                continue;

            yield return new RarEntry(entry, path);
        }
    }

    protected override IEnumerable<EntryBase> GetEntriesFromStream(Stream stream)
    {
        IRarArchive rar = RarArchive.OpenArchive(stream);
        foreach (IArchiveEntry entry in rar.Entries)
        {
            if (ShouldSkipEntry(entry.IsDirectory))
                continue;

            if (!ShouldInclude(entry.Key) || ShouldExclude(entry.Key))
                continue;

            yield return new RarEntry(entry, stream);
        }
    }
}
