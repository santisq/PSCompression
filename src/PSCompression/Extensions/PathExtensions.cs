using System;
using System.IO;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.PowerShell.Commands;
using PSCompression.Exceptions;

namespace PSCompression.Extensions;

public static class PathExtensions
{
    private static readonly Regex s_reNormalize = new(
        @"(?:^[a-z]:)?[\\/]+|(?<![\\/])$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex s_reEntryDir = new(
        @"[\\/]$",
        RegexOptions.Compiled | RegexOptions.RightToLeft);

    private const string _directorySeparator = "/";

    internal static string ResolvePath(this string path, PSCmdlet cmdlet)
    {
        string resolved = cmdlet.SessionState.Path.GetUnresolvedProviderPathFromPSPath(
            path: path,
            provider: out ProviderInfo provider,
            drive: out _);

        provider.Validate(path, throwOnInvalidProvider: true, cmdlet);
        return resolved;
    }

    internal static bool Validate(
        this ProviderInfo provider,
        string path,
        bool throwOnInvalidProvider,
        PSCmdlet cmdlet)
    {
        if (provider.ImplementingType == typeof(FileSystemProvider))
        {
            return true;
        }

        ErrorRecord error = provider.ToInvalidProviderError(path);

        if (throwOnInvalidProvider)
        {
            cmdlet.ThrowTerminatingError(error);
        }

        cmdlet.WriteError(error);
        return false;
    }

    internal static string GetParent(this string path) => Path.GetDirectoryName(path);

    internal static string AddExtensionIfMissing(this string path, string extension)
    {
        if (!path.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase)
            && !AlgorithmMappings.HasExtension(path))
        {
            path += extension;
        }

        return path;
    }

    internal static string NormalizeEntryPath(this string path) =>
        s_reNormalize.Replace(path, _directorySeparator).TrimStart('/');

    internal static string NormalizeFileEntryPath(this string path) =>
        NormalizeEntryPath(path).TrimEnd('/');

    internal static bool IsDirectoryPath(this string path) =>
        s_reEntryDir.IsMatch(path);

    public static string NormalizePath(this string path) =>
        s_reEntryDir.IsMatch(path)
            ? NormalizeEntryPath(path)
            : NormalizeFileEntryPath(path);
}
