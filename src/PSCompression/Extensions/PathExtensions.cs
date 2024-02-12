using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.PowerShell.Commands;
using static PSCompression.Exceptions.ExceptionHelpers;

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
        s_normalizedPaths ??= new();
        Collection<string> resolvedPaths;
        ProviderInfo provider;
        s_normalizedPaths.Clear();

        foreach (string path in paths)
        {
            if (isLiteral)
            {
                string resolvedPath = cmdlet.SessionState.Path
                    .GetUnresolvedProviderPathFromPSPath(path, out provider, out _);

                if (!provider.IsFileSystem())
                {
                    if (throwOnInvalidProvider)
                    {
                        cmdlet.ThrowTerminatingError(InvalidProviderError(path, provider));
                    }

                    cmdlet.WriteError(InvalidProviderError(path, provider));

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
                        cmdlet.WriteError(InvalidProviderError(
                            resolvedPath,
                            provider));

                        continue;
                    }

                    s_normalizedPaths.Add(resolvedPath);
                }
            }
            catch (Exception e)
            {
                cmdlet.WriteError(ResolvePathError(path, e));
            }
        }

        return s_normalizedPaths.ToArray();
    }

    internal static string NormalizePath(
        this string path,
        bool isLiteral,
        PSCmdlet cmdlet,
        bool throwOnInvalidProvider = false) =>
        NormalizePath(new[] { path }, isLiteral, cmdlet, throwOnInvalidProvider)
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
