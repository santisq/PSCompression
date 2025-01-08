using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using PSCompression.Extensions;
using PSCompression.Exceptions;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.Get, "ZipEntry", DefaultParameterSetName = "Path")]
[OutputType(typeof(ZipEntryDirectory), typeof(ZipEntryFile))]
[Alias("gezip")]
public sealed class GetZipEntryCommand : CommandWithPathBase
{
    private readonly List<ZipEntryBase> _output = [];

    private WildcardPattern[]? _includePatterns;

    private WildcardPattern[]? _excludePatterns;

    [Parameter]
    public ZipEntryType? Type { get; set; }

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
            _excludePatterns = Exclude
                .Select(e => new WildcardPattern(e, wpoptions))
                .ToArray();
        }

        if (Include is not null)
        {
            _includePatterns = Include
                .Select(e => new WildcardPattern(e, wpoptions))
                .ToArray();
        }
    }

    protected override void ProcessRecord()
    {
        foreach (string path in EnumerateResolvedPaths())
        {
            if (!path.IsArchive())
            {
                WriteError(ExceptionHelper.NotArchivePath(
                    path,
                    IsLiteral ? nameof(LiteralPath) : nameof(Path)));

                continue;
            }

            try
            {
                WriteObject(GetEntries(path), enumerateCollection: true);
            }
            catch (Exception exception)
            {
                WriteError(exception.ToOpenError(path));
            }
        }
    }

    private IEnumerable<ZipEntryBase> GetEntries(string path)
    {
        using ZipArchive zip = ZipFile.OpenRead(path);
        _output.Clear();

        foreach (ZipArchiveEntry entry in zip.Entries)
        {
            bool isDirectory = string.IsNullOrEmpty(entry.Name);

            if (ShouldSkipEntry(isDirectory))
            {
                continue;
            }

            if (!ShouldInclude(entry) || ShouldExclude(entry))
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

        return _output.ZipEntrySort();
    }

    private static bool MatchAny(
        ZipArchiveEntry entry,
        WildcardPattern[] patterns)
    {
        foreach (WildcardPattern pattern in patterns)
        {
            if (pattern.IsMatch(entry.FullName))
            {
                return true;
            }
        }

        return false;
    }

    private bool ShouldInclude(ZipArchiveEntry entry)
    {
        if (_includePatterns is null)
        {
            return true;
        }

        return MatchAny(entry, _includePatterns);
    }

    private bool ShouldExclude(ZipArchiveEntry entry)
    {
        if (_excludePatterns is null)
        {
            return false;
        }

        return MatchAny(entry, _excludePatterns);
    }

    private bool ShouldSkipEntry(bool isDirectory) =>
        isDirectory && Type is ZipEntryType.Archive
        || !isDirectory && Type is ZipEntryType.Directory;
}
