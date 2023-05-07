using System.ComponentModel;
using System.IO;
using System.Management.Automation;

namespace PSCompression;

#pragma warning disable IDE1006

[EditorBrowsable(EditorBrowsableState.Never)]
public static class _Format
{
    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetParentEntry(ZipEntryDirectory entry) =>
        string.Concat(entry.EntryRelativePath, $" @ Source: {entry.Source}");

    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetParentEntry(ZipEntryFile entry) =>
        string.Concat(Path.GetDirectoryName(entry.EntryRelativePath), $" @ Source: {entry.Source}");
}
