using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;
using Microsoft.PowerShell.Commands;

namespace PSCompression;

public static class Extensions
{
    private static readonly Regex s_reNormalize = new(@"[\\/]+|(?<![\\/])$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex s_reEntryDir = new(@"[\\/]$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly List<(string, ProviderInfo)> s_normalizedPaths = new();

    private const string _pathChar = "/";

    internal static string NormalizeEntryPath(this string path) =>
        s_reNormalize.Replace(path, _pathChar).TrimStart('/');

    internal static string NormalizeFileEntryPath(this string path) =>
        NormalizeEntryPath(path).TrimEnd('/');

    internal static bool IsDirectoryPath(this string path) =>
        s_reEntryDir.IsMatch(path);

    public static string NormalizePath(this string path) =>
        s_reEntryDir.IsMatch(path) ? NormalizeEntryPath(path) :
            NormalizeFileEntryPath(path);

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

    internal static ZipArchiveEntry CreateEntryFromFile(
        this ZipArchive zip,
        string entry,
        FileStream fileStream,
        CompressionLevel compressionLevel)
    {
        if (entry.IsDirectoryPath())
        {
            return zip.CreateEntry(entry);
        }

        fileStream.Seek(0, SeekOrigin.Begin);
        ZipArchiveEntry newentry = zip.CreateEntry(entry, compressionLevel);

        using (Stream stream = newentry.Open())
        {
            fileStream.CopyTo(stream);
        }

        return newentry;
    }
}
