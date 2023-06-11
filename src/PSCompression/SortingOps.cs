using System.IO;
using System.Linq;

namespace PSCompression;

internal static class SortingOps
{
    internal static int SortByParent(ZipEntryBase entry)
    {
        return Path.GetDirectoryName(entry.EntryRelativePath)
            .NormalizeEntryPath()
            .Count(e => e == '/');
    }
}
