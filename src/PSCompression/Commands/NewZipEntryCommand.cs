using System;
using System.Collections.Generic;
using System.IO;
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
    [ValidateNotNull]
    public string[]? Value { get; set; }

    [Parameter(Mandatory = true, Position = 0)]
    public string Destination { get; set; } = null!;

    [Parameter(Mandatory = true, Position = 1)]
    public string[] EntryPath { get; set; } = null!;

    [Parameter(ParameterSetName = "File", Position = 2)]
    [ValidateNotNullOrEmpty]
    public string? SourcePath { get; set; }

    [Parameter]
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    [Parameter(ParameterSetName = "Value")]
    [ArgumentCompleter(typeof(EncodingCompleter))]
    [EncodingTransformation]
    public Encoding Encoding { get; set; } = new UTF8Encoding();

    protected override void BeginProcessing()
    {
        (string? path, ProviderInfo? provider) = Destination.NormalizePath(isLiteral: true, this);

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

        Destination = path;

        try
        {
            _zip = ZipFile.Open(Destination, ZipArchiveMode.Update);

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

            using FileStream fileStream = File.OpenRead(sourcePath);

            foreach (string entry in EntryPath)
            {
                if (entry.IsDirectoryPath())
                {
                    _result.Add(CreateDirectoryEntry(entry, _zip));
                    continue;
                }

                fileStream.Seek(0, SeekOrigin.Begin);
                _result.Add(CreateEntryFromFile(entry, _zip, fileStream));
            }
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(ExceptionHelpers.ZipOpenError(Destination, e));
        }
    }

    protected override void ProcessRecord()
    {
        // no input from pipeline, go to end block
        if (Value is null || _zip is null)
        {
            return;
        }

        try
        {
            _writers ??= _result
                .Where(e => !string.IsNullOrEmpty(e.Name))
                .Select(e => new ZipContentWriter(_zip, e, Encoding))
                .ToArray();

            foreach (ZipContentWriter writer in _writers)
            {
                writer.WriteLines(Value);
            }
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(ExceptionHelpers.WriteError(_zip, e));
        }
    }

    protected override void EndProcessing()
    {
        try
        {
            if (_writers is not null)
            {
                foreach (ZipContentWriter writer in _writers)
                {
                    writer.Close();
                }
            }

            _zip?.Dispose();

            using ZipArchive zip = ZipFile.OpenRead(Destination);

            foreach (ZipArchiveEntry entry in _result)
            {
                if (string.IsNullOrEmpty(entry.Name))
                {
                    WriteObject(new ZipEntryDirectory(
                        zip.GetEntry(entry.FullName),
                        Destination));

                    continue;
                }

                WriteObject(new ZipEntryFile(
                    zip.GetEntry(entry.FullName),
                    Destination));
            }
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(ExceptionHelpers.ZipOpenError(Destination, e));
        }
    }

    private ZipArchiveEntry CreateDirectoryEntry(string entry, ZipArchive zip) =>
        zip.CreateEntry(entry.NormalizeEntryPath());

    private ZipArchiveEntry CreateFileEntry(string entry, ZipArchive zip) =>
        zip.CreateEntry(entry.NormalizeFileEntryPath(), CompressionLevel);

    private ZipArchiveEntry CreateEntryFromFile(
        string entry,
        ZipArchive zip,
        FileStream fileStream)
    {
        ZipArchiveEntry newentry = CreateFileEntry(entry, zip);

        using (Stream stream = newentry.Open())
        {
            fileStream.CopyTo(stream);
        }

        return newentry;
    }

    public void Dispose()
    {
        if (_writers is not null)
        {
            foreach (ZipContentWriter writer in _writers)
            {
                writer?.Dispose();
            }
        }

        _zip?.Dispose();
    }
}
