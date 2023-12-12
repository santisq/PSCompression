using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;

namespace PSCompression;

public static class ZipEntryExtensions
{
    private static readonly Regex s_reNormalize = new(@"[\\/]+|(?<![\\/])$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex s_reEntryDir = new(@"[\\/]$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly char[] s_InvalidFileNameChar = Path.GetInvalidFileNameChars();

    private static readonly char[] s_InvalidPathChar = Path.GetInvalidPathChars();

    private const string _pathChar = "/";

    internal static bool HasInvalidFileNameChar(this string name) =>
        name.IndexOfAny(s_InvalidFileNameChar) != -1;

    internal static bool HasInvalidPathChar(this string name) =>
        name.IndexOfAny(s_InvalidPathChar) != -1;

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

    internal static void ThrowIfNotFound(
        this ZipArchive zip,
        string path,
        string source,
        out ZipArchiveEntry entry)
    {
        if (!zip.TryGetEntry(path, out entry))
        {
            throw EntryNotFoundException.Create(path, source);
        }
    }

    internal static void ThrowIfDuplicate(
        this ZipArchive zip,
        string path,
        string source,
        out string normalizedPath)
    {
        normalizedPath = path.NormalizeFileEntryPath();
        if (zip.TryGetEntry(normalizedPath, out ZipArchiveEntry _))
        {
            throw DuplicatedEntryException.Create(normalizedPath, source);
        }
    }
}
