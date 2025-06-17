using System;
using System.ComponentModel;
using System.Globalization;
using System.Management.Automation;
using PSCompression.Abstractions;

namespace PSCompression.Internal;

#pragma warning disable IDE1006

[EditorBrowsable(EditorBrowsableState.Never)]
public static class _Format
{
    private static readonly CultureInfo _culture = CultureInfo.CurrentCulture;

    private readonly static string[] s_suffix =
    [
        "B",
        "KB",
        "MB",
        "GB",
        "TB",
        "PB",
        "EB",
        "ZB",
        "YB"
    ];

    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string? GetDirectoryPath(EntryBase entry) => entry.FormatDirectoryPath;

    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetFormattedDate(DateTime dateTime) =>
        string.Format(_culture, "{0,10:d} {0,8:t}", dateTime);

    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetFormattedLength(long length)
    {
        int index = 0;
        double len = length;

        while (len >= 1024)
        {
            len /= 1024;
            index++;
        }

        return $"{Math.Round(len, 2):0.00} {s_suffix[index],2}";
    }
}
