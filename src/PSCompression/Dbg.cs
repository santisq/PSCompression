using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

namespace PSCompression;

internal static class Dbg
{
    [Conditional("DEBUG")]
    public static void Assert(
        [DoesNotReturnIf(false)] bool condition,
        string? message = null,
        [CallerFilePath] string filePath = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        if (!condition)
        {
            string detail = string.IsNullOrEmpty(message)
                ? $"Assertion failed at {Path.GetFileName(filePath)}:{lineNumber}"
                : $"{message} ({Path.GetFileName(filePath)}:{lineNumber})";

            Debug.Assert(false, detail);
        }
    }
}
