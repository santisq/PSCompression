using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PSCompression;

[Cmdlet(VerbsCommon.Get, "ZipEntries", DefaultParameterSetName = "Path")]
[OutputType(typeof(ZipEntryDirectory), typeof(ZipEntryDirectory))]
[Alias("gezip")]
public sealed class GetZipEntriesCommand : PSCmdlet
{
    private bool _isLiteral;

    private string[] _paths = Array.Empty<string>();

    private WildcardPattern[]? _includePatterns;

    private WildcardPattern[]? _excludePatterns;

    private bool _withInclude;

    private bool _withExclude;

    private (string, ProviderInfo)[] NormalizePaths()
    {
        List<(string, ProviderInfo)> result = new();
        Collection<string> resolvedPaths;
        ProviderInfo provider;

        foreach (string path in _paths)
        {
            if (_isLiteral)
            {
                string resolvedPath = SessionState.Path.GetUnresolvedProviderPathFromPSPath(
                    path, out provider, out _);

                result.Add((resolvedPath, provider));
                continue;
            }

            try
            {
                resolvedPaths = GetResolvedProviderPathFromPSPath(path, out provider);

                foreach (string resolvedPath in resolvedPaths)
                {
                    result.Add((resolvedPath, provider));
                }
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(
                    e, "ResolvePath", ErrorCategory.NotSpecified, path));
            }
        }

        return result.ToArray();
    }

    [Parameter(
        ParameterSetName = "Path",
        Position = 0,
        Mandatory = true,
        ValueFromPipeline = true
    )]
    [SupportsWildcards]
    public string[] Path
    {
        get => _paths;
        set
        {
            _paths = value;
            _isLiteral = false;
        }
    }

    [Parameter(
        ParameterSetName = "LiteralPath",
        Mandatory = true,
        ValueFromPipelineByPropertyName = true
    )]
    [Alias("PSPath")]
    public string[] LiteralPath
    {
        get => _paths;
        set
        {
            _paths = value;
            _isLiteral = true;
        }
    }

    [Parameter]
    [ValidateSet("File", "Directory")]
    public string? EntryType { get; set; }

    [Parameter]
    [SupportsWildcards]
    public string[]? Include { get; set; }

    [Parameter]
    [SupportsWildcards]
    public string[]? Exclude { get; set; }

    protected override void BeginProcessing()
    {
        if (Exclude is null && Include is null)
        {
            return;
        }

        const WildcardOptions wpoptions =
            WildcardOptions.Compiled
            | WildcardOptions.CultureInvariant
            | WildcardOptions.IgnoreCase;

        if (Exclude is not null)
        {
            _withExclude = true;
            _excludePatterns = Exclude
                .Select(e => new WildcardPattern(e, wpoptions))
                .ToArray();
        }

        if (Include is not null)
        {
            _withInclude = true;
            _includePatterns = Include
                .Select(e => new WildcardPattern(e, wpoptions))
                .ToArray();
        }
    }

    protected override void ProcessRecord()
    {
        foreach ((string path, ProviderInfo provider) in NormalizePaths())
        {
            if (provider.ImplementingType != typeof(FileSystemProvider))
            {
                WriteError(new ErrorRecord(
                    new ArgumentException($"The resolved path '{path}' is not a FileSystem path but {provider.Name}."),
                    "PathNotFileSystem", ErrorCategory.InvalidArgument, path));

                continue;
            }

            try
            {
                if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
                {
                    WriteError(new ErrorRecord(
                        new ArgumentException($"Unable to get zip content because it is a directory: '{path}'."),
                        "PathIsDirectory", ErrorCategory.InvalidArgument, path));

                    continue;
                }
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(
                    e, "InvalidPath", ErrorCategory.InvalidArgument, path));

                continue;
            }

            try
            {
                using ZipArchive zip = ZipFile.OpenRead(path);

                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    bool isDirectory = string.IsNullOrEmpty(entry.Name);

                    if (_withInclude && !_includePatterns.Any(e => e.IsMatch(entry.FullName)))
                    {
                        continue;
                    }

                    if (_withExclude && _excludePatterns.Any(e => e.IsMatch(entry.FullName)))
                    {
                        continue;
                    }

                    if ((isDirectory && EntryType == "File") || (!isDirectory && EntryType == "Directory"))
                    {
                        continue;
                    }

                    if (isDirectory)
                    {
                        WriteObject(new ZipEntryDirectory(entry, path));
                        continue;
                    }

                    WriteObject(new ZipEntryFile(entry, path));
                }
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(
                    e, "ZipOpen", ErrorCategory.OpenError, path));
            }
        }
    }
}
