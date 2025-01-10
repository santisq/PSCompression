using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using PSCompression.Extensions;
using PSCompression.Exceptions;
using System.IO;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.Get, "ZipEntry", DefaultParameterSetName = "Path")]
[OutputType(typeof(ZipEntryDirectory), typeof(ZipEntryFile))]
[Alias("gezip")]
public sealed class GetZipEntryCommand : CommandWithPathBase
{
    [Parameter(
        ParameterSetName = "Stream",
        Position = 0,
        Mandatory = true,
        ValueFromPipeline = true,
        ValueFromPipelineByPropertyName = true)]
    [Alias("RawContentStream")]
    public Stream? Stream { get; set; }

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

        const WildcardOptions options = WildcardOptions.Compiled
            | WildcardOptions.CultureInvariant
            | WildcardOptions.IgnoreCase;

        if (Exclude is not null)
        {
            _excludePatterns = Exclude
                .Select(e => new WildcardPattern(e, options))
                .ToArray();
        }

        if (Include is not null)
        {
            _includePatterns = Include
                .Select(e => new WildcardPattern(e, options))
                .ToArray();
        }
    }

    protected override void ProcessRecord()
    {
        IEnumerable<ZipEntryBase> entries;
        if (Stream is not null)
        {
            ZipEntryBase CreateFromStream(ZipArchiveEntry entry, bool isDirectory) =>
                isDirectory
                    ? new ZipEntryDirectory(entry, Stream)
                    : new ZipEntryFile(entry, Stream);

            try
            {
                using (ZipArchive zip = new(Stream, ZipArchiveMode.Read, true))
                {
                    entries = GetEntries(zip, CreateFromStream);
                }
                WriteObject(entries, enumerateCollection: true);
                return;
            }
            catch (InvalidDataException exception)
            {
                ThrowTerminatingError(exception.ToInvalidZipArchive());
            }
            catch (Exception exception)
            {
                WriteError(exception.ToOpenError("InputStream"));
            }
        }

        foreach (string path in EnumerateResolvedPaths())
        {
            ZipEntryBase CreateFromFile(ZipArchiveEntry entry, bool isDirectory) =>
                isDirectory
                    ? new ZipEntryDirectory(entry, path)
                    : new ZipEntryFile(entry, path);

            if (!path.IsArchive())
            {
                WriteError(
                    ExceptionHelper.NotArchivePath(
                        path,
                        IsLiteral ? nameof(LiteralPath) : nameof(Path)));

                continue;
            }

            try
            {
                using (ZipArchive zip = ZipFile.OpenRead(path))
                {
                    entries = GetEntries(zip, CreateFromFile);
                }
                WriteObject(entries, enumerateCollection: true);
            }
            catch (InvalidDataException exception)
            {
                ThrowTerminatingError(exception.ToInvalidZipArchive());
            }
            catch (Exception exception)
            {
                WriteError(exception.ToOpenError(path));
            }
        }
    }

    private IEnumerable<ZipEntryBase> GetEntries(
        ZipArchive zip,
        Func<ZipArchiveEntry, bool, ZipEntryBase> createMethod)
    {
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

            _output.Add(createMethod(entry, isDirectory));
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
