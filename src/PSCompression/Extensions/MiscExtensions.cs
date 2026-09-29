using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Net;
using System.Security;
using PSCompression.Abstractions.Entries;

namespace PSCompression.Extensions;

internal static class MiscExtensions
{
    extension<TKey, TValue>(KeyValuePair<TKey, TValue> keyValue)
    {
        internal void Deconstruct(out TKey key, out TValue value)
        {
            key = keyValue.Key;
            value = keyValue.Value;
        }
    }

    extension(SecureString secureString)
    {
        internal string AsPlainText()
            => new NetworkCredential(string.Empty, secureString).Password;
    }

    extension(PSHost host)
    {
        [ExcludeFromCodeCoverage]
        internal string PromptForPassword(EntryBase entry)
        {
            host.UI.Write(
                $"Encrypted entry '{entry.RelativePath}' in '{entry.Source}' requires a password.\n" +
                "Use -Password <SecureString> to avoid this prompt in the future.\n\n" +
                "Enter password: ");

            return host.UI.ReadLineAsSecureString().AsPlainText();
        }

        [ExcludeFromCodeCoverage]
        internal string PromptForPassword(string message)
        {
            host.UI.Write(message + "\n" +
                "Use -Password <SecureString> to avoid this prompt in the future.\n\n" +
                "Enter password: ");

            return host.UI.ReadLineAsSecureString().AsPlainText();
        }
    }

    extension(StreamReader reader)
    {
        internal void ReadToEnd(PSCmdlet cmdlet) => cmdlet.WriteObject(reader.ReadToEnd());

        internal void ReadLines(PSCmdlet cmdlet)
        {
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                cmdlet.WriteObject(line);
            }
        }
    }

    extension(StreamWriter writer)
    {
        internal void WriteLines(string[] lines)
        {
            foreach (string line in lines)
            {
                writer.WriteLine(line);
            }
        }

        internal void WriteContent(string[] lines)
        {
            foreach (string line in lines)
            {
                writer.Write(line);
            }
        }
    }

    extension(InvocationInfo invocation)
    {
        internal bool HasBound(string parameter)
            => invocation.BoundParameters.ContainsKey(parameter);
    }

    extension(long size)
    {
        internal string CalculateRatio(long compressedSize)
        {
            float compressedRatio = (float)compressedSize / size;
            if (float.IsNaN(compressedRatio)) compressedRatio = 0;
            return string.Format("{0:F2}%", 100 - (compressedRatio * 100));
        }
    }
}
