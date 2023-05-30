using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace PSCompression.Internal;

#pragma warning disable IDE1006

[EditorBrowsable(EditorBrowsableState.Never)]
public static class _Format
{
    private readonly static Regex s_re;

    private const string _pathChar = "/";

    static _Format() => s_re = new Regex(
        @"[\\/]+|(?<![\\/])$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetParentEntry(ZipEntryDirectory entry) =>
        string.Concat(
            s_re.Replace(entry.EntryRelativePath, _pathChar),
            $" => {entry.Source}");

    [Hidden, EditorBrowsable(EditorBrowsableState.Never)]
    public static string GetParentEntry(ZipEntryFile entry) =>
        string.Concat(
            s_re.Replace(Path.GetDirectoryName(entry.EntryRelativePath), _pathChar),
            $" => {entry.Source}");
}
