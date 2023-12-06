using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PSCompression;

internal static class SortingOps
{
    private static string SortByParent(ZipEntryBase entry) =>
        Path.GetDirectoryName(entry.EntryRelativePath)
            .NormalizeEntryPath();

    private static int SortByLength(ZipEntryBase entry) =>
        entry.EntryRelativePath.Count(e => e == '/');

    private static string SortByName(ZipEntryBase entry) =>
        entry.EntryName;

    internal static IEnumerable<ZipEntryBase> ZipEntrySort(this IEnumerable<ZipEntryBase> zip) =>
        zip
            .OrderBy(SortByParent)
            .ThenBy(SortByLength)
            .ThenBy(SortByName);
}
