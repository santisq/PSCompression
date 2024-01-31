using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace PSCompression;

public static class ZipEntryExtensions
{
    private static readonly Regex s_reNormalize = new(
        @"(?:^[A-Za-z]:)?[\\/]+|(?<![\\/])$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex s_reEntryDir = new(
        @"[\\/]$",
        RegexOptions.Compiled | RegexOptions.RightToLeft);

    private static readonly Regex s_reChangeDirName = new(
        @"[^/]+(?=/$)",
        RegexOptions.Compiled | RegexOptions.RightToLeft);

    private const string _pathChar = "/";

    internal static string NormalizeEntryPath(this string path) =>
        s_reNormalize.Replace(path, _pathChar).TrimStart('/');

    internal static string NormalizeFileEntryPath(this string path) =>
        NormalizeEntryPath(path).TrimEnd('/');

    internal static bool IsDirectoryPath(this string path) =>
        s_reEntryDir.IsMatch(path);

    public static string NormalizePath(this string path) =>
        s_reEntryDir.IsMatch(path) ? NormalizeEntryPath(path) :
            NormalizeFileEntryPath(path);

    internal static string RelativeTo(this DirectoryInfo directory, int length) =>
        NormalizeEntryPath(directory.FullName.Substring(length) + _pathChar);

    internal static string RelativeTo(this FileInfo directory, int length) =>
        NormalizeFileEntryPath(directory.FullName.Substring(length));

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
        return string.Join(
            _pathChar,
            normalized.Substring(0, normalized.Length - file.Name.Length),
            newname);
    }

    internal static string ChangeName(
        this ZipEntryDirectory directory,
        string newname) =>
        s_reChangeDirName.Replace(
            directory.RelativePath.NormalizePath(),
            newname);

    internal static string ChangePath(
        this ZipArchiveEntry entry,
        string oldPath,
        string newPath) =>
        string.Concat(newPath, entry.FullName.Remove(0, oldPath.Length));
}
