using System.IO;
using ICSharpCode.SharpZipLib.Tar;
using PSCompression.Abstractions;
using PSCompression.Extensions;

namespace PSCompression;

public sealed class TarEntryDirectory : TarEntryBase
{
    public override EntryType Type => EntryType.Directory;

    internal override string? FormatDirectoryPath { get; }

    internal TarEntryDirectory(TarEntry entry, string source)
        : base(entry, source)
    {
        Name = entry.GetDirectoryName();
        FormatDirectoryPath = $"/{RelativePath.NormalizeEntryPath()}";
    }

    internal TarEntryDirectory(TarEntry entry, Stream? stream)
        : base(entry, stream)
    {
        Name = entry.GetDirectoryName();
        FormatDirectoryPath = $"/{RelativePath.NormalizeEntryPath()}";
    }
}
