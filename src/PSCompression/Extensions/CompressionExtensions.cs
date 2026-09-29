using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Tar;
using ICSharpCode.SharpZipLib.Zip;
using PSCompression.Enum;
using PSCompression.FormatHandlers.Rar;
using PSCompression.FormatHandlers.Zip;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;
using SharpCompress.Compressors.LZMA;
using ZstdSharp;
using SharpCompressors = SharpCompress.Compressors;

namespace PSCompression.Extensions;

internal static partial class CompressionExtensions
{
    private const string DirectorySeparator = "/";

#if NET8_0_OR_GREATER
    [GeneratedRegex(
        @"[^/]+(?=/$)",
        RegexOptions.Compiled | RegexOptions.RightToLeft)]
    private static partial Regex GetDirectoryName();

    private static readonly Regex s_reGetDirName = GetDirectoryName();
#else
    private static readonly Regex s_reGetDirName = new(
        @"[^/]+(?=/$)",
        RegexOptions.Compiled | RegexOptions.RightToLeft);
#endif

    extension(DirectoryInfo directory)
    {
        internal string RelativeTo(int length) =>
            $"{directory.FullName.Substring(length)}{DirectorySeparator}".NormalizeEntryPath();
    }

    extension(FileInfo file)
    {
        internal string RelativeTo(int length) =>
            file.FullName.Substring(length).NormalizeFileEntryPath();
    }

    extension(ZipArchive zip)
    {
        internal ZipArchiveEntry CreateEntryFromFile(
            string entry,
            FileStream fileStream,
            CompressionLevel compressionLevel)
        {
            if (entry.EndsWith("/") || entry.EndsWith("\\"))
            {
                return zip.CreateEntry(entry);
            }

            fileStream.Position = 0;
            ZipArchiveEntry newentry = zip.CreateEntry(entry, compressionLevel);

            using (Stream stream = newentry.Open())
            {
                fileStream.CopyTo(stream);
            }

            return newentry;
        }

        internal bool TryGetEntry(string path, [NotNullWhen(true)] out ZipArchiveEntry? entry)
        {
            entry = zip.GetEntry(path);
            return entry is not null;
        }
    }

    extension(ICSharpCode.SharpZipLib.Zip.ZipFile zip)
    {
        internal bool TryGetEntry(string path, [NotNullWhen(true)] out ZipEntry? entry)
        {
            entry = zip.GetEntry(path);
            return entry is not null;
        }
    }

    extension(ZipEntryFile file)
    {
        internal string GetNewName(string newname)
        {
            string normalized = file.RelativePath.NormalizePath();
            return normalized.Contains(DirectorySeparator)
                ? string.Join(
                    DirectorySeparator,
                    normalized.Substring(0, normalized.Length - file.Name!.Length - 1),
                    newname)
                : newname;
        }
    }

    extension(ZipEntryDirectory directory)
    {
        internal string ChangeName(string newname)
            => s_reGetDirName.Replace(directory.RelativePath.NormalizePath(), newname);
    }

    extension(ZipEntry entry)
    {
        internal string GetDirectoryName() => s_reGetDirName.Match(entry.Name).Value;
    }

    extension(TarEntry entry)
    {
        internal string GetDirectoryName() => s_reGetDirName.Match(entry.Name).Value;
    }

    extension(Stream stream)
    {
        internal BrotliSharpLib.BrotliStream AsBrotliCompressedStream(CompressionLevel compressionLevel)
        {
            BrotliSharpLib.BrotliStream brotli = new(stream, CompressionMode.Compress);
            brotli.SetQuality(compressionLevel switch
            {
                CompressionLevel.NoCompression => 0,
                CompressionLevel.Fastest => 1,
                _ => 11
            });

            return brotli;
        }

        internal BZip2OutputStream AsBZip2CompressedStream(CompressionLevel compressionLevel)
        {
            int blockSize = compressionLevel switch
            {
                CompressionLevel.NoCompression => 1,
                CompressionLevel.Fastest => 2,
                _ => 9
            };

            return new BZip2OutputStream(stream, blockSize);
        }

        internal Stream AsZstCompressedStream(CompressionLevel compressionLevel)
        {
            int level = compressionLevel switch
            {
                CompressionLevel.NoCompression => 1,
                CompressionLevel.Fastest => 3,
                _ => 19
            };

            return new CompressionStream(stream, level);
        }

        internal LZipStream AsLzCompressedStream() =>
            LZipStream.Create(stream, SharpCompressors.CompressionMode.Compress);
    }

    extension(Algorithm algorithm)
    {
        internal Stream ToCompressedStream(Stream stream, CompressionLevel compressionLevel)
            => algorithm switch
            {
                Algorithm.gz => new GZipStream(stream, compressionLevel),
                Algorithm.zst => stream.AsZstCompressedStream(compressionLevel),
                Algorithm.lz => stream.AsLzCompressedStream(),
                Algorithm.bz2 => stream.AsBZip2CompressedStream(compressionLevel),
                _ => stream
            };

        internal Stream FromCompressedStream(Stream stream)
            => algorithm switch
            {
                Algorithm.gz => new GZipStream(stream, CompressionMode.Decompress),
                Algorithm.zst => new DecompressionStream(stream),
                Algorithm.lz => LZipStream.Create(stream, SharpCompressors.CompressionMode.Decompress),
                Algorithm.bz2 => new BZip2InputStream(stream),
                _ => stream
            };
    }

    extension(TarOutputStream stream)
    {
        internal void CreateTarEntry(string entryName, DateTime modTime, long size)
        {
            TarEntry entry = TarEntry.CreateTarEntry(entryName);
            entry.TarHeader.Size = size;
            entry.TarHeader.ModTime = modTime;
            stream.PutNextEntry(entry);
        }
    }

    extension(TarInputStream tar)
    {
        internal IEnumerable<TarEntry> EnumerateEntries()
        {
            TarEntry? entry;
            while ((entry = tar.GetNextEntry()) is not null)
                yield return entry;
        }
    }

    extension(IRarArchive rar)
    {
        internal IArchiveEntry GetEntry(RarEntry entry) =>
            rar.Entries.FirstOrDefault(e => e.Key == entry.RelativePath) ?? throw new IOException(
                $"The entry '{entry.RelativePath}' was not found in RAR archive '{entry.Source}'.");
    }
}
