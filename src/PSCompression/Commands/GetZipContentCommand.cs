using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PSCompression;

[Cmdlet(VerbsCommon.Get, "ZipContent", DefaultParameterSetName = "Path")]
[OutputType(typeof(ZipEntryBase))]
[Alias("gczip")]
public sealed class GetZipContentCommand : PSCmdlet
{
    private bool _isLiteral;

    private string[] _paths = Array.Empty<string>();

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
    public SwitchParameter Force;

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
            catch(PipelineStoppedException)
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
                    WriteObject(new ZipEntryBase(entry, path));
                }
            }
            catch(PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(
                    e, "ZipOpen", ErrorCategory.NotSpecified, path));
            }
        }
    }
}
