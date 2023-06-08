using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.PowerShell.Commands;

namespace PSCompression;

internal static class PSCompressionExtensions
{
    private static readonly Regex s_reNormalize = new(@"[\\/]+|(?<![\\/])$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex s_reEntryDir = new(@"[\\/]$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly List<(string, ProviderInfo)> s_normalizedPaths = new();

    private const string _pathChar = "/";

    internal static string ToNormalizedEntryPath(this string path) =>
        s_reNormalize.Replace(path, _pathChar);

    internal static string ToNormalizedFileEntryPath(this string path) =>
        ToNormalizedEntryPath(path).TrimEnd('/');

    internal static bool IsDirectoryPath(this string path) =>
        s_reEntryDir.IsMatch(path);

    internal static (string, ProviderInfo)[] NormalizePath(
        this string[] paths, bool isLiteral, PSCmdlet cmdlet)
    {
        Collection<string> resolvedPaths;
        ProviderInfo provider;
        s_normalizedPaths.Clear();

        foreach (string path in paths)
        {
            if (isLiteral)
            {
                string resolvedPath = cmdlet.SessionState.Path.GetUnresolvedProviderPathFromPSPath(
                    path, out provider, out _);

                s_normalizedPaths.Add((resolvedPath, provider));
                continue;
            }

            try
            {
                resolvedPaths = cmdlet.GetResolvedProviderPathFromPSPath(path, out provider);

                foreach (string resolvedPath in resolvedPaths)
                {
                    s_normalizedPaths.Add((resolvedPath, provider));
                }
            }
            catch (Exception e)
            {
                cmdlet.WriteError(ExceptionHelpers.ResolvePathError(path, e));
            }
        }

        return s_normalizedPaths.ToArray();
    }

    internal static (string?, ProviderInfo?) NormalizePath(
        this string path, bool isLiteral, PSCmdlet cmdlet) =>
        NormalizePath(new string[1] { path }, isLiteral, cmdlet).FirstOrDefault();

    internal static bool AssertFileSystem(this ProviderInfo provider) =>
        provider.ImplementingType == typeof(FileSystemProvider);

    internal static bool AssertArchive(this string path) =>
        !File.GetAttributes(path).HasFlag(FileAttributes.Directory);

    internal static bool AssertDirectory(this string path) =>
        File.GetAttributes(path).HasFlag(FileAttributes.Directory);
}