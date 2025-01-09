using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Management.Automation;
using PSCompression.Exceptions;
using PSCompression.Extensions;

namespace PSCompression;

[EditorBrowsable(EditorBrowsableState.Never)]
public abstract class CommandWithPathBase : PSCmdlet
{
    protected string[] _paths = [];

    protected bool IsLiteral
    {
        get => MyInvocation.BoundParameters.ContainsKey("LiteralPath");
    }

    [Parameter(
        ParameterSetName = "Path",
        Position = 0,
        Mandatory = true,
        ValueFromPipeline = true)]
    [SupportsWildcards]
    public virtual string[] Path
    {
        get => _paths;
        set => _paths = value;
    }

    [Parameter(
        ParameterSetName = "LiteralPath",
        Mandatory = true,
        ValueFromPipelineByPropertyName = true)]
    [Alias("PSPath")]
    public virtual string[] LiteralPath
    {
        get => _paths;
        set => _paths = value;
    }

    protected IEnumerable<string> EnumerateResolvedPaths()
    {
        Collection<string> resolvedPaths;
        ProviderInfo provider;

        foreach (string path in _paths)
        {
            if (IsLiteral)
            {
                string resolved = SessionState.Path.GetUnresolvedProviderPathFromPSPath(
                    path: path,
                    provider: out provider,
                    drive: out _);

                if (provider.Validate(path, throwOnInvalidProvider: false, this))
                {
                    yield return resolved;
                }

                continue;
            }

            try
            {
                resolvedPaths = GetResolvedProviderPathFromPSPath(path, out provider);
            }
            catch (Exception exception)
            {
                WriteError(exception.ToResolvePathError(path));
                continue;
            }


            foreach (string resolvedPath in resolvedPaths)
            {
                if (provider.Validate(path, throwOnInvalidProvider: false, this))
                {
                    yield return resolvedPath;
                }
            }
        }
    }

    protected string ResolvePath(string path) => path.ResolvePath(this);
}
