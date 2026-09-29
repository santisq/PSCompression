using System;
using System.IO;
using System.Management.Automation;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using Microsoft.PowerShell.Commands;

namespace PSCompression.Extensions;

public static partial class PathExtensions
{
#if NETCOREAPP
    [GeneratedRegex(
        @"(?:^[a-z]:)?[\\/]+|(?<![\\/])$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex NormalizeRegex();

    private static readonly Regex s_reNormalize = NormalizeRegex();
#else
    private static readonly Regex s_reNormalize = new(
        @"(?:^[a-z]:)?[\\/]+|(?<![\\/])$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);
#endif

    private const string DirectorySeparator = "/";

    extension(string path)
    {
        public string NormalizePath()
            => path.EndsWith("/") || path.EndsWith("\\")
                ? NormalizeEntryPath(path) : NormalizeFileEntryPath(path);

        internal string ResolvePath(PSCmdlet cmdlet)
        {
            string resolved = cmdlet.SessionState.Path.GetUnresolvedProviderPathFromPSPath(
                path: path,
                provider: out ProviderInfo provider,
                drive: out _);

            provider.Validate(path, throwOnInvalidProvider: true, cmdlet);
            return resolved;
        }

        internal string AddExtensionIfMissing(string extension)
        {
            if (!path.EndsWith(extension, StringComparison.InvariantCultureIgnoreCase))
            {
                path += extension;
            }

            return path;
        }

        internal string NormalizeEntryPath()
            => s_reNormalize
                .Replace(path, DirectorySeparator)
                .TrimStart('/');

        internal string NormalizeFileEntryPath()
            => NormalizeEntryPath(path).TrimEnd('/');
    }

    extension(ProviderInfo provider)
    {
        internal bool Validate(
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
    }

    extension(FileSystemInfo info)
    {
        internal PSObject AppendPSProperties()
        {
            string? parent = info is DirectoryInfo dir
                ? dir.Parent?.FullName
                : Unsafe.As<FileInfo>(info).DirectoryName;

            return info.AppendPSProperties(parent);
        }

        internal PSObject AppendPSProperties(string? parent)
        {
            const string Provider = @"Microsoft.PowerShell.Core\FileSystem::";
            PSObject pso = PSObject.AsPSObject(info);
            pso.Properties.Add(new PSNoteProperty("PSPath", $"{Provider}{info.FullName}"));
            pso.Properties.Add(new PSNoteProperty("PSParentPath", $"{Provider}{parent}"));
            return pso;
        }
    }
}
