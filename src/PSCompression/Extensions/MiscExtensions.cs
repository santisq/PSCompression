using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Net;
using System.Security;

namespace PSCompression.Extensions;

internal static class MiscExtensions
{
    internal static void Deconstruct<TKey, TValue>(
        this KeyValuePair<TKey, TValue> keyv,
        out TKey key,
        out TValue value)
    {
        key = keyv.Key;
        value = keyv.Value;
    }

    internal static string AsPlainText(this SecureString secureString) =>
        new NetworkCredential(string.Empty, secureString).Password;

    [ExcludeFromCodeCoverage]
    internal static string PromptForPassword(this ZipEntryFile entry, PSHost host)
    {
        host.UI.Write(
            $"Encrypted entry '{entry.RelativePath}' in '{entry.Source}' requires a password.\n" +
            "Tip: Use -Password <SecureString> to avoid this prompt in the future.\n" +
            "Enter password: ");

        return host.UI.ReadLineAsSecureString().AsPlainText();
    }

    internal static void ReadToEnd(this StreamReader reader, PSCmdlet cmdlet)
        => cmdlet.WriteObject(reader.ReadToEnd());

    internal static void ReadLines(this StreamReader reader, PSCmdlet cmdlet)
    {
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            cmdlet.WriteObject(line);
        }
    }

    internal static void WriteLines(this StreamWriter writer, string[] lines)
    {
        foreach (string line in lines)
        {
            writer.WriteLine(line);
        }
    }

    internal static void WriteContent(this StreamWriter writer, string[] lines)
    {
        foreach (string line in lines)
        {
            writer.Write(line);
        }
    }
}
