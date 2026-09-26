using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using ICSharpCode.SharpZipLib.Zip;
using PSCompression.Abstractions;
using PSCompression.Extensions;

namespace PSCompression;

public sealed class ZipEntryDirectory : ZipEntryBase
{
    private const StringComparison Comparer = StringComparison.InvariantCultureIgnoreCase;

    public override EntryType Type => EntryType.Directory;

    internal override string? FormatDirectoryPath { get; }

    internal ZipEntryDirectory(ZipEntry entry, string source)
        : base(entry, source)
    {
        Name = entry.GetDirectoryName();
        FormatDirectoryPath = $"/{RelativePath.NormalizeEntryPath()}";
    }

    internal ZipEntryDirectory(ZipEntry entry, Stream? stream)
        : base(entry, stream)
    {
        Name = entry.GetDirectoryName();
        FormatDirectoryPath = $"/{RelativePath.NormalizeEntryPath()}";
    }

    internal IEnumerable<ZipArchiveEntry> GetChilds(ZipArchive zip) =>
        zip.Entries.Where(e =>
            !string.Equals(e.FullName, RelativePath, Comparer)
            && e.FullName.StartsWith(RelativePath, Comparer));
}
