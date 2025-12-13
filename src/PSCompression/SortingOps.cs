using System.Collections.Generic;
using System.IO;
using System.Linq;
using PSCompression.Abstractions;
using PSCompression.Extensions;

namespace PSCompression;

internal static class SortingOps
{
    private static string? SortByParent(EntryBase entry) =>
        Path.GetDirectoryName(entry.RelativePath)?.NormalizeEntryPath();

    private static int SortByLength(EntryBase entry) =>
        entry.RelativePath.Count(e => e == '/');

    private static string SortByName(EntryBase entry) => entry.Name!;

    private static EntryType SortByType(EntryBase entry) => entry.Type;

    internal static IEnumerable<EntryBase> ToEntrySort(
        this IEnumerable<EntryBase> entryCollection)
        => entryCollection
            .OrderBy(SortByParent)
            .ThenBy(SortByType)
            .ThenBy(SortByLength)
            .ThenBy(SortByName);
}
