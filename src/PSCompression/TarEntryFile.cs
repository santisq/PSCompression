using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Tar;
using PSCompression.Abstractions;
using PSCompression.Extensions;

namespace PSCompression;

public sealed class TarEntryFile : TarEntryBase
{
    private readonly Algorithm _algorithm;

    public string BaseName => Path.GetFileNameWithoutExtension(Name);

    public string Extension => Path.GetExtension(RelativePath);

    public override EntryType Type => EntryType.Archive;

    internal TarEntryFile(TarEntry entry, string source, Algorithm algorithm)
        : base(entry, source)
    {
        _algorithm = algorithm;
    }

    internal TarEntryFile(TarEntry entry, Stream? stream, Algorithm algorithm)
        : base(entry, stream)
    {
        _algorithm = algorithm;
    }

    protected override string GetFormatDirectoryPath() =>
        $"/{Path.GetDirectoryName(RelativePath).NormalizeEntryPath()}";

    internal bool GetContentStream(Stream stream)
    {
        Stream? sourceStream = null;
        Stream? decompressedStream = null;
        TarInputStream? tar = null;

        try
        {
            sourceStream = _stream ?? File.OpenRead(Source);
            sourceStream.Seek(0, SeekOrigin.Begin);
            decompressedStream = _algorithm.FromCompressedStream(sourceStream);
            tar = new(decompressedStream, Encoding.UTF8);

            TarEntry? entry = tar
                .EnumerateEntries()
                .FirstOrDefault(e => e.Name == RelativePath);

            if (entry is null or { Size: 0 })
            {
                return false;
            }

            tar.CopyTo(stream, (int)entry.Size);
            stream.Seek(0, SeekOrigin.Begin);
            return true;
        }
        finally
        {
            if (!FromStream)
            {
                tar?.Dispose();
                decompressedStream?.Dispose();
                sourceStream?.Dispose();
            }
        }
    }
}
