using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Text.RegularExpressions;
using BrotliSharpLib;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Tar;
using SharpCompress.Compressors.LZMA;
using ZstdSharp;
using SharpCompressors = SharpCompress.Compressors;

namespace PSCompression.Extensions;

internal static class CompressionExtensions
{
    private static readonly Regex s_reGetDirName = new(
        @"[^/]+(?=/$)",
        RegexOptions.Compiled | RegexOptions.RightToLeft);

    private const string _directorySeparator = "/";

    internal static string RelativeTo(this DirectoryInfo directory, int length) =>
        (directory.FullName.Substring(length) + _directorySeparator).NormalizeEntryPath();

    internal static string RelativeTo(this FileInfo file, int length) =>
        file.FullName.Substring(length).NormalizeFileEntryPath();

    internal static ZipArchiveEntry CreateEntryFromFile(
        this ZipArchive zip,
        string entry,
        FileStream fileStream,
        CompressionLevel compressionLevel)
    {
        if (entry.EndsWith("/") || entry.EndsWith("\\"))
        {
            return zip.CreateEntry(entry);
        }

        fileStream.Seek(0, SeekOrigin.Begin);
        ZipArchiveEntry newentry = zip.CreateEntry(entry, compressionLevel);

        using (Stream stream = newentry.Open())
        {
            fileStream.CopyTo(stream);
        }

        return newentry;
    }

    internal static bool TryGetEntry(
        this ZipArchive zip,
        string path,
        [NotNullWhen(true)] out ZipArchiveEntry? entry)
        => (entry = zip.GetEntry(path)) is not null;

    internal static string ChangeName(
        this ZipEntryFile file,
        string newname)
    {
        string normalized = file.RelativePath.NormalizePath();

        if (normalized.IndexOf(_directorySeparator) == -1)
        {
            return newname;
        }

        return string.Join(
            _directorySeparator,
            normalized.Substring(0, normalized.Length - file.Name.Length - 1),
            newname);
    }

    internal static string ChangeName(
        this ZipEntryDirectory directory,
        string newname)
        => s_reGetDirName.Replace(
            directory.RelativePath.NormalizePath(),
            newname);

    internal static string GetDirectoryName(this ZipArchiveEntry entry)
        => s_reGetDirName.Match(entry.FullName).Value;

    internal static string GetDirectoryName(this TarEntry entry)
        => s_reGetDirName.Match(entry.Name).Value;

    internal static void WriteAllTextToPipeline(this StreamReader reader, PSCmdlet cmdlet)
        => cmdlet.WriteObject(reader.ReadToEnd());

    internal static void WriteLinesToPipeline(this StreamReader reader, PSCmdlet cmdlet)
    {
        string line;
        while ((line = reader.ReadLine()) is not null)
        {
            cmdlet.WriteObject(line);
        }
    }

    internal static void WriteLines(this StreamWriter writer, string[] lines)
    {
        foreach (string line in lines)
        {
            writer.WriteLine(line);
        }
    }

    internal static void WriteContent(this StreamWriter writer, string[] lines)
    {
        foreach (string line in lines)
        {
            writer.Write(line);
        }
    }

    internal static BrotliStream AsBrotliCompressedStream(
        this Stream stream,
        CompressionLevel compressionLevel)
    {
        BrotliStream brotli = new(stream, CompressionMode.Compress);
        brotli.SetQuality(compressionLevel switch
        {
            CompressionLevel.NoCompression => 0,
            CompressionLevel.Fastest => 1,
            _ => 11
        });

        return brotli;
    }

    internal static BZip2OutputStream AsBZip2CompressedStream(
        this Stream stream,
        CompressionLevel compressionLevel)
    {
        int blockSize = compressionLevel switch
        {
            CompressionLevel.NoCompression => 1,
            CompressionLevel.Fastest => 2,
            _ => 9
        };

        return new BZip2OutputStream(stream, blockSize);
    }

    internal static CompressionStream AsZstCompressedStream(
        this Stream stream,
        CompressionLevel compressionLevel)
    {
        int level = compressionLevel switch
        {
            CompressionLevel.NoCompression => 1,
            CompressionLevel.Fastest => 3,
            _ => 19
        };

        return new CompressionStream(stream, level);
    }

    internal static LZipStream AsLzCompressedStream(this Stream outputStream) =>
        new(outputStream, SharpCompressors.CompressionMode.Compress);

    internal static Stream ToCompressedStream(
        this Algorithm algorithm,
        Stream stream,
        CompressionLevel compressionLevel)
        => algorithm switch
        {
            Algorithm.gz => new GZipStream(stream, compressionLevel),
            Algorithm.zst => stream.AsZstCompressedStream(compressionLevel),
            Algorithm.lz => stream.AsLzCompressedStream(),
            Algorithm.bz2 => stream.AsBZip2CompressedStream(compressionLevel),
            _ => stream
        };

    internal static Stream FromCompressedStream(
        this Algorithm algorithm,
        Stream stream)
        => algorithm switch
        {
            Algorithm.gz => new GZipStream(stream, CompressionMode.Decompress),
            Algorithm.zst => new DecompressionStream(stream),
            Algorithm.lz => new LZipStream(stream, SharpCompressors.CompressionMode.Decompress),
            Algorithm.bz2 => new BZip2InputStream(stream),
            _ => stream
        };

    internal static void CreateTarEntry(
        this TarOutputStream stream,
        string entryName,
        DateTime modTime,
        long size)
    {
        TarEntry entry = TarEntry.CreateTarEntry(entryName);
        entry.TarHeader.Size = size;
        entry.TarHeader.ModTime = modTime;
        stream.PutNextEntry(entry);
    }

    internal static IEnumerable<TarEntry> EnumerateEntries(this TarInputStream tar)
    {
        TarEntry? entry;
        while ((entry = tar.GetNextEntry()) is not null)
        {
            yield return entry;
        }
    }
}
