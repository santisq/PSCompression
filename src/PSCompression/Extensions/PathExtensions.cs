using System;
using System.IO;
using System.Management.Automation;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.PowerShell.Commands;
using PSCompression.Exceptions;

namespace PSCompression.Extensions;

public static class PathExtensions
{
    private static readonly Regex s_reNormalize = new(
        @"(?:^[a-z]:)?[\\/]+|(?<![\\/])$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private const string _directorySeparator = "/";

    public static string NormalizePath(this string path) =>
        path.EndsWith("/") || path.EndsWith("\\")
            ? NormalizeEntryPath(path)
            : NormalizeFileEntryPath(path);

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

    internal static string AddExtensionIfMissing(this string path, string extension)
    {
        if (!path.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
        {
            path += extension;
        }

        return path;
    }

    internal static string NormalizeEntryPath(this string path) =>
        s_reNormalize.Replace(path, _directorySeparator).TrimStart('/');

    internal static string NormalizeFileEntryPath(this string path) =>
        NormalizeEntryPath(path).TrimEnd('/');

    internal static void Create(this DirectoryInfo dir, bool force)
    {
        if (force || !dir.Exists)
        {
            dir.Create();
            return;
        }

        throw new IOException($"The directory '{dir.FullName}' already exists.");
    }

    internal static PSObject AppendPSProperties(this FileSystemInfo info)
    {
        string? parent = info is DirectoryInfo dir
            ? dir.Parent?.FullName
            : Unsafe.As<FileInfo>(info).DirectoryName;

        return info.AppendPSProperties(parent);
    }

    internal static PSObject AppendPSProperties(this FileSystemInfo info, string? parent)
    {
        const string provider = @"Microsoft.PowerShell.Core\FileSystem::";
        PSObject pso = PSObject.AsPSObject(info);
        pso.Properties.Add(new PSNoteProperty("PSPath", $"{provider}{info.FullName}"));
        pso.Properties.Add(new PSNoteProperty("PSParentPath", $"{provider}{parent}"));
        return pso;
    }
}
