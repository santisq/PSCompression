using System.IO;
using System.Linq;

namespace PSCompression;

internal static class SortingOps
{
    internal static string SortByParent(ZipEntryBase entry) =>
        Path.GetDirectoryName(entry.EntryRelativePath)
            .NormalizeEntryPath();

    internal static int SortByLength(ZipEntryBase entry) =>
        entry.EntryRelativePath.Count(e => e == '/');

    internal static string SortByName(ZipEntryBase entry) =>
        entry.EntryName;
}
