using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PSCompression;

internal static class PSCompressionExtensions
{
    private readonly static string[] s_suffix =
    {
        " B",
        "KB",
        "MB",
        "GB",
        "TB",
        "PB",
        "EB",
        "ZB",
        "YB"
    };

    private static readonly Regex s_reNormalize = new(@"[\\/]+|(?<![\\/])$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex s_reEntryDir = new(@"[\\/]$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private const string _pathChar = "/";

    internal static string ToNormalizedEntryPath(this string path) =>
        s_reNormalize.Replace(path, _pathChar);

    internal static string ToNormalizedFileEntryPath(this string path) =>
        ToNormalizedEntryPath(path).TrimEnd('/');

    internal static string ToFormattedDate(this DateTime date) =>
        string.Format(CultureInfo.CurrentCulture, "{0,10:d} {0,8:t}", date);

    internal static bool IsDirectoryPath(this string path) =>
        s_reEntryDir.IsMatch(path);

    internal static string ToFormattedLength(this long length)
    {
        int index = 0;
        double len = length;

        while (len >= 1024)
        {
            len /= 1024;
            index++;
        }

        return $"{Math.Round(len, 2):0.00} {s_suffix[index]}";
    }
}
