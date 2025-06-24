using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using PSCompression.Abstractions;
using PSCompression.Extensions;

namespace PSCompression;

public sealed class ZipEntryDirectory : ZipEntryBase
{
    private const StringComparison _comparer = StringComparison.InvariantCultureIgnoreCase;

    public override EntryType Type => EntryType.Directory;

    internal ZipEntryDirectory(ZipArchiveEntry entry, string source)
        : base(entry, source)
    {
        Name = entry.GetDirectoryName();
    }

    internal ZipEntryDirectory(ZipArchiveEntry entry, Stream? stream)
        : base(entry, stream)
    {
        Name = entry.GetDirectoryName();
    }

    internal IEnumerable<ZipArchiveEntry> GetChilds(ZipArchive zip) =>
        zip.Entries.Where(e =>
            !string.Equals(e.FullName, RelativePath, _comparer)
            && e.FullName.StartsWith(RelativePath, _comparer));

    protected override string GetFormatDirectoryPath() =>
        $"/{RelativePath.NormalizeEntryPath()}";
}
