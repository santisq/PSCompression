using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace PSCompression;

public class EncodingCompleter : IArgumentCompleter
{
    private static readonly string[] s_encodingSet =
    {
        "ascii",
        "bigendianUtf32",
        "unicode",
        "utf8",
        "utf8NoBOM",
        "bigendianUnicode",
        "oem",
        "utf8BOM",
        "utf32",
        "ansi"
    };

    public IEnumerable<CompletionResult> CompleteArgument(
        string commandName,
        string parameterName,
        string wordToComplete,
        CommandAst commandAst,
        IDictionary fakeBoundParameters)
    {
        foreach (string encoding in s_encodingSet)
        {
            yield return new CompletionResult(encoding);
        }
    }
}
