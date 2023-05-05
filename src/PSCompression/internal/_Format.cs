using System.ComponentModel;
using System.IO;
using System.Management.Automation;

namespace PSCompression;

#pragma warning disable IDE1006

[EditorBrowsable(EditorBrowsableState.Never)]
public static class _Format
{
    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetParentEntry(ZipEntry entry)
    {
        return string.Concat(entry.EntryType == "Directory" ?
            entry.EntryRelativePath : Path.GetDirectoryName(entry.EntryRelativePath),
            $" @ {entry.Source}");
    }
}
