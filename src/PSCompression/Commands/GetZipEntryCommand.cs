using System.Collections.Generic;
using System.IO.Compression;
using System.Management.Automation;
using System.IO;
using PSCompression.Abstractions;

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
        using (ZipArchive zip = ZipFile.OpenRead(path))
        {
            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                bool isDirectory = string.IsNullOrEmpty(entry.Name);

                if (ShouldSkipEntry(isDirectory))
                {
                    continue;
                }

                if (!ShouldInclude(entry.Name) || ShouldExclude(entry.Name))
                {
                    continue;
                }

                entries.Add(isDirectory
                    ? new ZipEntryDirectory(entry, path)
                    : new ZipEntryFile(entry, path));
            }
        }

        return [.. entries];
    }

    protected override IEnumerable<EntryBase> GetEntriesFromStream(Stream stream)
    {
        List<EntryBase> entries = [];
        using (ZipArchive zip = new(stream, ZipArchiveMode.Read, true))
        {
            foreach (ZipArchiveEntry entry in zip.Entries)
            {
                bool isDirectory = string.IsNullOrEmpty(entry.Name);

                if (ShouldSkipEntry(isDirectory))
                {
                    continue;
                }

                if (!ShouldInclude(entry.Name) || ShouldExclude(entry.Name))
                {
                    continue;
                }

                entries.Add(isDirectory
                    ? new ZipEntryDirectory(entry, stream)
                    : new ZipEntryFile(entry, stream));
            }
        }

        return [.. entries];
    }
}
