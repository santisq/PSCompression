using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;

namespace PSCompression;

public sealed class ZipEntryDirectory : ZipEntryBase
{
    private const StringComparison _comparer = StringComparison.InvariantCultureIgnoreCase;

    public override ZipEntryType Type => ZipEntryType.Directory;

    internal ZipEntryDirectory(ZipArchiveEntry entry, string source)
        : base(entry, source)
    {
        Name = entry.GetDirectoryName();
    }

    internal string Rename(
        string newname,
        ZipArchive zip) =>
        Move(
            path: RelativePath,
            destination: this.ChangeName(newname),
            source: Source,
            zip: zip);

        // foreach (ZipArchiveEntry entry in GetChilds(zip))
        // {
        //     Move(
        //         path: entry.FullName,
        //         destination: string.Concat(
        //             destination,
        //             entry.FullName.Remove(0, RelativePath.Length)),
        //         source: Source,
        //         zip: zip);
        // }

    internal IEnumerable<ZipArchiveEntry> GetChilds(ZipArchive zip) =>
        zip.Entries.Where(e =>
            !string.Equals(e.FullName, RelativePath, _comparer)
            && e.FullName.StartsWith(RelativePath, _comparer));
}
