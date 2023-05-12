using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Get, "ZipEntries", DefaultParameterSetName = "Path")]
[OutputType(typeof(ZipEntryDirectory), typeof(ZipEntryDirectory))]
[Alias("gezip")]
public sealed class GetZipEntriesCommand : CommandsBase
{
    private bool _isLiteral;

    private string[] _paths = Array.Empty<string>();

    private WildcardPattern[]? _includePatterns;

    private WildcardPattern[]? _excludePatterns;

    private bool _withInclude;

    private bool _withExclude;

    private readonly List<ZipEntryBase> _output = new();

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
        foreach ((string path, ProviderInfo provider) in NormalizePaths(_paths, _isLiteral))
        {
            if (!ValidatePath(path, provider))
            {
                continue;
            }

            try
            {
                using (ZipArchive zip = ZipFile.OpenRead(path))
                {
                    _output.Clear();

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
                            _output.Add(new ZipEntryDirectory(entry, path));
                            continue;
                        }

                        _output.Add(new ZipEntryFile(entry, path));
                    }
                }

                WriteObject(_output.ToArray(), enumerateCollection: true);
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
