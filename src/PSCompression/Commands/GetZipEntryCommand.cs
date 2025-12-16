using System.Collections.Generic;
using System.Management.Automation;
using System.IO;
using PSCompression.Abstractions;
using ICSharpCode.SharpZipLib.Zip;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.Get, "ZipEntry", DefaultParameterSetName = "Path")]
[OutputType(typeof(ZipEntryDirectory), typeof(ZipEntryFile))]
[Alias("zipge")]
public sealed class GetZipEntryCommand : GetEntryCommandBase
{
    internal override ArchiveType ArchiveType => ArchiveType.zip;

    protected override IEnumerable<EntryBase> GetEntriesFromFile(string path)
    {
        List<EntryBase> entries = [];
        using (ZipFile zip = new(path))
        {
            foreach (ZipEntry entry in zip)
            {
                if (ShouldSkipEntry(entry.IsDirectory))
                {
                    continue;
                }

                if (!ShouldInclude(entry.Name) || ShouldExclude(entry.Name))
                {
                    continue;
                }

                entries.Add(entry.IsDirectory
                    ? new ZipEntryDirectory(entry, path)
                    : new ZipEntryFile(entry, path));
            }
        }

        return [.. entries];
    }

    protected override IEnumerable<EntryBase> GetEntriesFromStream(Stream stream)
    {
        List<EntryBase> entries = [];
        using (ZipFile zip = new(stream, leaveOpen: true))
        {
            foreach (ZipEntry entry in zip)
            {
                if (ShouldSkipEntry(entry.IsDirectory))
                {
                    continue;
                }

                if (!ShouldInclude(entry.Name) || ShouldExclude(entry.Name))
                {
                    continue;
                }

                entries.Add(entry.IsDirectory
                    ? new ZipEntryDirectory(entry, stream)
                    : new ZipEntryFile(entry, stream));
            }
        }

        return [.. entries];
    }
}
