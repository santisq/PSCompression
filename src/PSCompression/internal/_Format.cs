using System;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;

namespace PSCompression.Internal;

#pragma warning disable IDE1006

[EditorBrowsable(EditorBrowsableState.Never)]
public static class _Format
{
    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetParentEntry(ZipEntryDirectory entry) =>
        entry.EntryRelativePath.ToNormalizedEntryPath();

    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetParentEntry(ZipEntryFile entry) =>
        string.Concat(
            Path.GetDirectoryName(entry.EntryRelativePath).ToNormalizedEntryPath(),
            $" => {entry.Source}");

    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetFormattedDate(DateTime date) =>
        date.ToFormattedDate();

    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetFormattedLength(long length) =>
        length.ToFormattedLength();
}
