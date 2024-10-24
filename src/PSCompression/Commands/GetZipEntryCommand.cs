﻿using System;
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
public sealed class GetZipEntryCommand : PSCmdlet
{
    private bool _isLiteral;

    private bool _withInclude;

    private bool _withExclude;

    private string[] _paths = [];

    private readonly List<ZipEntryBase> _output = new();

    private WildcardPattern[]? _includePatterns;

    private WildcardPattern[]? _excludePatterns;

    [Parameter(
        ParameterSetName = "Path",
        Position = 0,
        Mandatory = true,
        ValueFromPipeline = true)]
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
        ValueFromPipelineByPropertyName = true)]
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
        foreach (string path in _paths.NormalizePath(_isLiteral, this))
        {
            if (!path.IsArchive())
            {
                WriteError(ExceptionHelpers.NotArchivePathError(
                    path,
                    _isLiteral ? nameof(LiteralPath) : nameof(Path)));

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

            if (SkipEntryType(isDirectory))
            {
                continue;
            }

            if (SkipInclude(entry.FullName))
            {
                continue;
            }

            if (SkipExclude(entry.FullName))
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

    private bool SkipInclude(string path) =>
        _withInclude && !_includePatterns.Any(e => e.IsMatch(path));

    private bool SkipExclude(string path) =>
        _withExclude && _excludePatterns.Any(e => e.IsMatch(path));

    private bool SkipEntryType(bool isdir) =>
        (isdir && Type is ZipEntryType.Archive) || (!isdir && Type is ZipEntryType.Directory);
}
