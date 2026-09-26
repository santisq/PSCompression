using System.Collections.Generic;
using System.Linq;
using PSCompression.Abstractions;
using PSCompression.Enum;

namespace PSCompression.FormatHandlers.Common;

internal static class EntrySortingExtensions
{
    private static string? GetParentPathKey(EntryBase entry) =>
        entry.FormatDirectoryPath;

    private static EntryType GetTypeKey(EntryBase entry) => entry.Type;

    private static int GetDepthKey(EntryBase entry) =>
        entry.RelativePath.Count(e => e == '/');

    private static string GetNameKey(EntryBase entry) => entry.Name!;

    extension(IEnumerable<EntryBase> entries)
    {
        internal IEnumerable<EntryBase> SortEntries() =>
            entries
                .OrderBy(GetParentPathKey)
                .ThenBy(GetTypeKey)
                .ThenBy(GetDepthKey)
                .ThenBy(GetNameKey);
    }
}
