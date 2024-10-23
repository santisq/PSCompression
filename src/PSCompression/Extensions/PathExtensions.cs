using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
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

    [ThreadStatic]
    private static List<string>? s_normalizedPaths;

    internal static string[] NormalizePath(
        this string[] paths,
        bool isLiteral,
        PSCmdlet cmdlet,
        bool throwOnInvalidProvider = false)
    {
        s_normalizedPaths ??= [];
        Collection<string> resolvedPaths;
        ProviderInfo provider;
        s_normalizedPaths.Clear();

        foreach (string path in paths)
        {
            if (isLiteral)
            {
                string resolvedPath = cmdlet
                    .SessionState.Path
                    .GetUnresolvedProviderPathFromPSPath(path, out provider, out _);

                if (!provider.IsFileSystem())
                {
                    if (throwOnInvalidProvider)
                    {
                        cmdlet.ThrowTerminatingError(provider.ToInvalidProviderError(path));
                    }

                    cmdlet.WriteError(provider.ToInvalidProviderError(path));
                    continue;
                }

                s_normalizedPaths.Add(resolvedPath);
                continue;
            }

            try
            {
                resolvedPaths = cmdlet.GetResolvedProviderPathFromPSPath(path, out provider);

                foreach (string resolvedPath in resolvedPaths)
                {
                    if (!provider.IsFileSystem())
                    {
                        cmdlet.WriteError(provider.ToInvalidProviderError(resolvedPath));
                        continue;
                    }

                    s_normalizedPaths.Add(resolvedPath);
                }
            }
            catch (Exception exception)
            {

                cmdlet.WriteError(exception.ToResolvePathError(path));
            }
        }

        return [.. s_normalizedPaths];
    }

    internal static string NormalizePath(
        this string path,
        bool isLiteral,
        PSCmdlet cmdlet,
        bool throwOnInvalidProvider = false) =>
        NormalizePath([path], isLiteral, cmdlet, throwOnInvalidProvider)
            .FirstOrDefault();

    internal static bool IsFileSystem(this ProviderInfo provider) =>
        provider.ImplementingType == typeof(FileSystemProvider);

    internal static bool IsArchive(this string path) =>
        !File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    internal static string GetParent(this string path) =>
        Path.GetDirectoryName(path);

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
