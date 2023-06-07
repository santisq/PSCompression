using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.New, "ZipEntry", DefaultParameterSetName = "Value")]
[OutputType(typeof(ZipEntryDirectory), typeof(ZipEntryFile))]
public sealed class NewZipEntryCommand : PSCmdlet
{
    private readonly List<ZipEntryBase> _result = new();

    private ZipArchive? _zip;

    [Parameter(ValueFromPipeline = true, ParameterSetName = "Value")]
    public object? Value { get; set; }

    [Parameter(ParameterSetName = "File")]
    public string? SourcePath { get; set; }

    [Parameter(Mandatory = true, Position = 0)]
    public string ZipPath { get; set; } = null!;

    [Parameter(Mandatory = true, Position = 1)]
    public string[] EntryPath { get; set; } = null!;

    [Parameter]
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    protected override void BeginProcessing()
    {
        (string? path, ProviderInfo? provider) = ZipPath.NormalizePath(isLiteral: true, this);

        if (path is null || provider is null)
        {
            return;
        }

        if (!provider.AssertFileSystem())
        {
            ThrowTerminatingError(ExceptionHelpers.NotFileSystemPathError(path, provider));
        }

        if (!path.AssertArchive())
        {
            ThrowTerminatingError(ExceptionHelpers.NotArchivePathError(path));
        }

        ZipPath = path;

        try
        {
            // WriteObject(CreateEntries(path), enumerateCollection: true);
            _zip = ZipFile.Open(path, ZipArchiveMode.Update);

            if (SourcePath is null)
            {
                return;
            }

            (string? sourcePath, ProviderInfo? sourceProvider) = SourcePath.NormalizePath(isLiteral: true, this);

            if (sourcePath is null || sourceProvider is null)
            {
                return;
            }

            if (!sourceProvider.AssertFileSystem())
            {
                ThrowTerminatingError(ExceptionHelpers.NotFileSystemPathError(sourcePath, sourceProvider));
            }

            if (!sourcePath.AssertArchive())
            {
                ThrowTerminatingError(ExceptionHelpers.NotArchivePathError(sourcePath));
            }

            SourcePath = sourcePath;

            foreach (string entry in EntryPath)
            {
                if (entry.IsDirectoryPath())
                {
                    _result.Add(CreateDirectoryEntry(entry, ZipPath, _zip));
                    continue;
                }

                _result.Add(CreateEntryFromFile(entry, ZipPath, _zip, SourcePath));
            }
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(ExceptionHelpers.ZipOpenError(ZipPath, e));
        }
    }

    private ZipEntryDirectory CreateDirectoryEntry(string entry, string source, ZipArchive zip) =>
        new(zip.CreateEntry(entry.ToNormalizedEntryPath(), CompressionLevel), source);

    private ZipEntryFile CreateFileEntry(string entry, string source, ZipArchive zip) =>
        new(zip.CreateEntry(entry.ToNormalizedFileEntryPath(), CompressionLevel), source);

    private ZipEntryFile CreateEntryFromFile(string entry, string source, ZipArchive zip, string file) =>
        new(zip.CreateEntryFromFile(file, entry.ToNormalizedFileEntryPath(), CompressionLevel), source);
}
