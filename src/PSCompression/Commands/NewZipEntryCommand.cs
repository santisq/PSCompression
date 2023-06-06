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
    public string SourceZip { get; set; } = null!;

    [Parameter(Mandatory = true, Position = 1)]
    public string[] EntryPath { get; set; } = null!;

    [Parameter]
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    protected override void BeginProcessing()
    {
        (string path, ProviderInfo provider) = NormalizePath(SourceZip, isLiteral: true);

        if (!ValidatePath(path, provider))
        {
            return;
        }

        try
        {
            // WriteObject(CreateEntries(path), enumerateCollection: true);
            _zip = ZipFile.Open(path, ZipArchiveMode.Update);

            if (SourcePath is null)
            {
                return;
            }


        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(new ErrorRecord(
                e, "ZipOpen", ErrorCategory.OpenError, path));
        }
    }

    private ZipEntryDirectory CreateDirectoryEntry(string entry, string source, ZipArchive zip) =>
        new(zip.CreateEntry(entry.ToNormalizedEntryPath(), CompressionLevel), source);

    private ZipEntryFile CreateFileEntry(string entry, string source, ZipArchive zip) =>
        new(zip.CreateEntry(entry.ToNormalizedFileEntryPath(), CompressionLevel), source);

    private ZipEntryFile CreateEntryFromFile(string entry, string source, ZipArchive zip, string file) =>
        new(zip.CreateEntryFromFile(file, entry.ToNormalizedFileEntryPath(), CompressionLevel), source);

    private ZipEntryFile[]? CreateEntriesFromFile(string file, string source, ZipArchive zip)
    {
        (string normalizedfile, ProviderInfo provider) = NormalizePath(file, isLiteral: true);

        if (!ValidatePath(normalizedfile, provider))
        {
            return;
        }

        return EntryPath.Select(e => CreateEntryFromFile(e, source, zip, normalizedfile)).ToArray();
    }

    private ZipEntryBase[] CreateEntries(string path)
    {

        _result.Clear();

        foreach (string entryPath in EntryRelativePath)
        {
            if (entryPath.IsDirectoryPath())
            {
                _result.Add(new ZipEntryDirectory(
                    zip.CreateEntry(entryPath.ToNormalizedEntryPath(), CompressionLevel), path));

                continue;
            }

            _result.Add(new ZipEntryFile(
                zip.CreateEntry(entryPath.ToNormalizedFileEntryPath(), CompressionLevel), path));
        }

        return _result.ToArray();
    }
}
