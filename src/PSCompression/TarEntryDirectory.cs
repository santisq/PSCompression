using System.IO;
using ICSharpCode.SharpZipLib.Tar;
using PSCompression.Abstractions;
using PSCompression.Extensions;

namespace PSCompression;

public sealed class TarEntryDirectory : TarEntryBase
{
    internal TarEntryDirectory(TarEntry entry, string source)
        : base(entry, source)
    {
        Name = entry.GetDirectoryName();
    }

    internal TarEntryDirectory(TarEntry entry, Stream? stream)
        : base(entry, stream)
    {
        Name = entry.GetDirectoryName();
    }

    public override EntryType Type => EntryType.Directory;

    protected override string GetFormatDirectoryPath() => $"/{RelativePath.NormalizeEntryPath()}";
}
