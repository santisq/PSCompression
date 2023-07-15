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
        directory.FullName.Substring(length) + _pathChar;

    internal static string RelativeTo(this FileInfo directory, int length) =>
        directory.FullName.Substring(length);

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
}
