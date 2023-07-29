using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;
using System;
using System.Runtime.InteropServices;

namespace PSCompression;

public sealed class EncodingCompleter : IArgumentCompleter
{
    private static readonly string[] s_encodingSet;

    static EncodingCompleter()
    {
        List<string> set = new(new string[9]
        {
            "ascii",
            "bigendianUtf32",
            "unicode",
            "utf8",
            "utf8NoBOM",
            "bigendianUnicode",
            "oem",
            "utf8BOM",
            "utf32"
        });

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            set.Add("ansi");
        }

        s_encodingSet = set.ToArray();
    }

    public IEnumerable<CompletionResult> CompleteArgument(
        string commandName,
        string parameterName,
        string wordToComplete,
        CommandAst commandAst,
        IDictionary fakeBoundParameters)
    {
        foreach (string encoding in s_encodingSet)
        {
            if (encoding.StartsWith(wordToComplete, StringComparison.InvariantCultureIgnoreCase))
            {
                yield return new CompletionResult(encoding);
            }
        }
    }
}
