using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using Microsoft.PowerShell.Commands;

namespace PSCompression;

public abstract class CommandsBase : PSCmdlet
{
    protected CommandsBase()
    { }

    protected (string, ProviderInfo)[] NormalizePaths(string[] paths, bool isLiteral)
    {
        List<(string, ProviderInfo)> result = new();
        Collection<string> resolvedPaths;
        ProviderInfo provider;

        foreach (string path in paths)
        {
            if (isLiteral)
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

    protected bool ValidatePath(string path, ProviderInfo provider)
    {
        if (provider.ImplementingType != typeof(FileSystemProvider))
        {
            WriteError(new ErrorRecord(
                new ArgumentException($"The resolved path '{path}' is not a FileSystem path but {provider.Name}."),
                "PathNotFileSystem", ErrorCategory.InvalidArgument, path));

            return false;
        }

        try
        {
            if (File.GetAttributes(path).HasFlag(FileAttributes.Directory))
            {
                WriteError(new ErrorRecord(
                    new ArgumentException($"Unable to get zip content because it is a directory: '{path}'."),
                    "PathIsDirectory", ErrorCategory.InvalidArgument, path));

                return false;
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

            return false;
        }

        return true;
    }
}
