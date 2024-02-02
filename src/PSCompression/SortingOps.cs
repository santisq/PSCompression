using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PSCompression;

internal static class SortingOps
{
    private static string SortByParent(ZipEntryBase entry) =>
        Path.GetDirectoryName(entry.RelativePath)
            .NormalizeEntryPath();

    private static int SortByLength(ZipEntryBase entry) =>
        entry.RelativePath.Count(e => e == '/');

    private static string SortByName(ZipEntryBase entry) =>
        entry.Name;

    private static ZipEntryType SortByType(ZipEntryBase entry) =>
        entry.Type;

    internal static IEnumerable<ZipEntryBase> ZipEntrySort(
        this IEnumerable<ZipEntryBase> zip) => zip
            .OrderBy(SortByParent)
            .ThenBy(SortByType)
            .ThenBy(SortByLength)
            .ThenBy(SortByName);
}
