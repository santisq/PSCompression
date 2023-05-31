using System.Text.RegularExpressions;

namespace PSCompression;

internal static class PSCompressionExtensions
{
    private static readonly Regex s_reNormalize = new(@"[\\/]+|(?<![\\/])$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex s_reEntryDir = new(@"[\\/]$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private const string _pathChar = "/";

    internal static string ToNormalizedEntryPath(this string path) =>
        s_reNormalize.Replace(path, _pathChar);

    internal static string ToNormalizedFileEntryPath(this string path) =>
        ToNormalizedEntryPath(path).TrimEnd('/');

    internal static bool IsDirectoryPath(this string path) =>
        s_reEntryDir.IsMatch(path);
}
