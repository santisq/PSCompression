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

    }

    // internal override string Move(
    //     string destination,
    //     ZipArchive zip)
    // {

    // }

    internal string Rename(
        string newname,
        ZipArchive zip)
    {
        string destination = this.ChangeName(newname);

        foreach (ZipArchiveEntry entry in GetChilds(zip))
        {
            Move(
                path: entry.FullName,
                destination: entry.FullName.Replace(RelativePath, destination),
                source: Source,
                zip: zip);
        }

        return Move(
            path: RelativePath,
            destination: destination,
            source: Source,
            zip: zip);
    }

    internal IEnumerable<ZipArchiveEntry> GetChilds(ZipArchive zip) =>
        zip.Entries.Where(e =>
            !string.Equals(e.FullName, RelativePath, _comparer)
            && e.FullName.StartsWith(RelativePath, _comparer));
}
