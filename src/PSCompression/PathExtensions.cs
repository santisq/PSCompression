using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PSCompression;

internal static class PathExtensions
{
    private static readonly List<string> s_normalizedPaths = new();

    internal static string GetParent(this string path) =>
        Path.GetDirectoryName(path);

    internal static string GetLeaf(this string path) =>
        Path.GetFileName(path);

    internal static string[] NormalizePath(
        this string[] paths,
        bool isLiteral,
        PSCmdlet cmdlet,
        bool shouldthrow = false)
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

                if (!provider.IsFileSystem())
                {
                    if (shouldthrow)
                    {
                        cmdlet.ThrowTerminatingError(ExceptionHelpers
                            .InvalidProviderError(path, provider));
                    }

                    cmdlet.WriteError(ExceptionHelpers
                        .InvalidProviderError(path, provider));

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
                        cmdlet.WriteError(ExceptionHelpers.InvalidProviderError(
                            resolvedPath, provider));
                        continue;
                    }

                    s_normalizedPaths.Add(resolvedPath);
                }
            }
            catch (Exception e)
            {
                cmdlet.WriteError(ExceptionHelpers.ResolvePathError(path, e));
            }
        }

        return s_normalizedPaths.ToArray();
    }

    internal static string NormalizePath(
        this string path,
        bool isLiteral,
        PSCmdlet cmdlet,
        bool shouldthrow = false) =>
        NormalizePath(new[] { path }, isLiteral, cmdlet, shouldthrow)
            .FirstOrDefault();

    internal static bool IsFileSystem(this ProviderInfo provider) =>
        provider.ImplementingType == typeof(FileSystemProvider);

    internal static bool IsArchive(this string path) =>
        !File.GetAttributes(path).HasFlag(FileAttributes.Directory);
}
