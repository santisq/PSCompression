using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace PSCompression.Extensions;

internal static class ZipEntryExtensions
{
    private static readonly Regex s_reGetDirName = new(
        @"[^/]+(?=/$)",
        RegexOptions.Compiled | RegexOptions.RightToLeft);

    private const string _directorySeparator = "/";

    internal static string RelativeTo(this DirectoryInfo directory, int length) =>
        (directory.FullName.Substring(length) + _directorySeparator).NormalizeEntryPath();

    internal static string RelativeTo(this FileInfo directory, int length) =>
        directory.FullName.Substring(length).NormalizeFileEntryPath();

    internal static ZipArchiveEntry CreateEntryFromFile(
        this ZipArchive zip,
        string entry,
        FileStream fileStream,
        CompressionLevel compressionLevel)
    {
        if (entry.IsDirectoryPath())
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
        out ZipArchiveEntry entry) =>
        (entry = zip.GetEntry(path)) is not null;

    internal static string Move(
        this ZipEntryBase entry,
        string destination,
        ZipArchive zip) =>
        ZipEntryBase.Move(
            path: entry.RelativePath,
            destination: destination,
            source: entry.Source,
            zip: zip);

    internal static (string, bool) ExtractTo(
        this ZipEntryBase entryBase,
        ZipArchive zip,
        string destination,
        bool overwrite)
    {
        destination = Path.GetFullPath(
            Path.Combine(destination, entryBase.RelativePath));

        if (string.IsNullOrEmpty(entryBase.Name))
        {
            Directory.CreateDirectory(destination);
            return (destination, false);
        }

        string parent = Path.GetDirectoryName(destination);

        if (!Directory.Exists(parent))
        {
            Directory.CreateDirectory(parent);
        }

        ZipArchiveEntry entry = zip.GetEntry(entryBase.RelativePath);
        entry.ExtractToFile(destination, overwrite);
        return (destination, true);
    }

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
        string newname) =>
        s_reGetDirName.Replace(
            directory.RelativePath.NormalizePath(),
            newname);

    internal static string ChangePath(
        this ZipArchiveEntry entry,
        string oldPath,
        string newPath) =>
        string.Concat(newPath, entry.FullName.Remove(0, oldPath.Length));

    internal static string GetDirectoryName(this ZipArchiveEntry entry) =>
        s_reGetDirName.Match(entry.FullName).Value;
}
