using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Management.Automation;

namespace PSCompression.Internal;

#pragma warning disable IDE1006

[EditorBrowsable(EditorBrowsableState.Never)]
public static class _Format
{
    private readonly static string[] s_suffix =
    {
        "B",
        "KB",
        "MB",
        "GB",
        "TB",
        "PB",
        "EB",
        "ZB",
        "YB"
    };

    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetDirectoryPath(ZipEntryBase entry)
    {
        if (entry is ZipEntryDirectory)
        {
            return entry.RelativePath.NormalizeEntryPath();
        }

        string path = Path.GetDirectoryName(entry.RelativePath)
            .NormalizeEntryPath();

        if (string.IsNullOrEmpty(path))
        {
            return "/";
        }

        return path;
    }

    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetFormattedDate(DateTime date) =>
        string.Format(CultureInfo.CurrentCulture, "{0,10:d} {0,8:t}", date);

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
