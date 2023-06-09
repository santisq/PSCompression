using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

[Cmdlet(VerbsCommon.New, "ZipEntry", DefaultParameterSetName = "Value")]
[OutputType(typeof(ZipEntryDirectory), typeof(ZipEntryFile))]
public sealed class NewZipEntryCommand : PSCmdlet, IDisposable
{
    private readonly List<ZipArchiveEntry> _result = new();

    private ZipArchive? _zip;

    private ZipContentWriter[]? _writers;

    [Parameter(ValueFromPipeline = true, ParameterSetName = "Value")]
    public string[]? Value { get; set; }

    [Parameter(Mandatory = true, Position = 0)]
    public string ZipPath { get; set; } = null!;

    [Parameter(Mandatory = true, Position = 1)]
    public string[] EntryPath { get; set; } = null!;

    [Parameter(ParameterSetName = "File", Position = 2)]
    public string? SourcePath { get; set; }

    [Parameter]
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    [Parameter(ParameterSetName = "Value")]
    [ArgumentCompleter(typeof(EncodingCompleter))]
    [EncodingTransformation]
    public Encoding Encoding { get; set; } = new UTF8Encoding();

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
            _zip = ZipFile.Open(ZipPath, ZipArchiveMode.Update);

            if (SourcePath is null)
            {
                // We can create the entries here and go the process block
                foreach (string entry in EntryPath)
                {
                    if (entry.IsDirectoryPath())
                    {
                        _result.Add(CreateDirectoryEntry(entry, _zip));
                        continue;
                    }

                    _result.Add(CreateFileEntry(entry, _zip));
                }

                return;
            }

            // Create Entries from file here
            (string? sourcePath, ProviderInfo? sourceProvider) = SourcePath
                .NormalizePath(isLiteral: true, this);

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
                    _result.Add(CreateDirectoryEntry(entry, _zip));
                    continue;
                }

                _result.Add(CreateEntryFromFile(entry, _zip));
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

    protected override void ProcessRecord()
    {
        // no input from pipeline, go to end block
        if (Value is null || _zip is null)
        {
            return;
        }

        _writers ??= _result
            .Where(e => !string.IsNullOrEmpty(e.Name))
            .Select(e =>
            {
                try
                {
                    new ZipContentWriter(_zip, e, Encoding);
                }
                catch (PipelineStoppedException)
                {
                    throw;
                }

            })
            .ToArray();

        foreach (ZipContentWriter writer in _writers)
        {
            try
            {
                writer.WriteLines(Value);
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteError(ExceptionHelpers.WriteError(writer, e));
            }
        }
    }

    protected override void EndProcessing()
    {
        if (_writers is not null)
        {
            foreach (ZipContentWriter writer in _writers)
            {
                writer.Dispose();
            }
        }

        if (_zip is not null)
        {
            _zip.Dispose();
            _zip = ZipFile.OpenRead(ZipPath);
        }

        foreach (ZipArchiveEntry entry in _result)
        {
            if (entry is ZipEntryFile fileEntry)
            {
                try
                {
                    fileEntry.Refresh();
                }
                catch (PipelineStoppedException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    WriteError(ExceptionHelpers.StreamOpenError(fileEntry, e));
                }
            }
        }

        WriteObject(_result.ToArray(), enumerateCollection: true);
    }

    private ZipArchiveEntry CreateDirectoryEntry(string entry, ZipArchive zip) =>
        zip.CreateEntry(entry.ToNormalizedEntryPath());

    private ZipArchiveEntry CreateFileEntry(string entry, ZipArchive zip) =>
        zip.CreateEntry(entry.ToNormalizedFileEntryPath(), CompressionLevel);

    private ZipArchiveEntry CreateEntryFromFile(string entry, ZipArchive zip) =>
        zip.CreateEntryFromFile(SourcePath, entry.ToNormalizedFileEntryPath(), CompressionLevel);

    public void Dispose()
    {
        _cache?.Dispose();
        _zip?.Dispose();
    }
}
