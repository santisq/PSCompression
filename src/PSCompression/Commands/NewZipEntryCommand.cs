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
    private readonly List<ZipArchiveEntry> _entries = new();

    private ZipArchive? _zip;

    private ZipContentWriter[]? _writers;

    private string[] _entryPath = Array.Empty<string>();

    [Parameter(ValueFromPipeline = true, ParameterSetName = "Value")]
    [ValidateNotNull]
    public string[]? Value { get; set; }

    [Parameter(Mandatory = true, Position = 0)]
    public string Destination { get; set; } = null!;

    [Parameter(Mandatory = true, Position = 1)]
    public string[] EntryPath
    {
        get => _entryPath;
        set => _entryPath = value.Select(e => e.NormalizePath()).ToArray();
    }

    [Parameter(ParameterSetName = "File", Position = 2)]
    [ValidateNotNullOrEmpty]
    public string? SourcePath { get; set; }

    [Parameter]
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    [Parameter(ParameterSetName = "Value")]
    [ArgumentCompleter(typeof(EncodingCompleter))]
    [EncodingTransformation]
    public Encoding Encoding { get; set; } = new UTF8Encoding();

    [Parameter]
    public SwitchParameter Force { get; set; }

    protected override void BeginProcessing()
    {
        string? path = Destination.NormalizePath(isLiteral: true, this);

        if (path is null)
        {
            return;
        }

        if (!path.IsArchive())
        {
            ThrowTerminatingError(
                ExceptionHelpers.NotArchivePathError(path));
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
                    ZipArchiveEntry? zipEntry = _zip.GetEntry(entry);

                    if (zipEntry is not null)
                    {
                        if (!Force.IsPresent)
                        {
                            WriteError(ExceptionHelpers.DuplicatedEntryError(
                                entry, Destination));

                            continue;
                        }

                        zipEntry.Delete();
                    }

                    _entries.Add(_zip.CreateEntry(entry, CompressionLevel));
                }

                return;
            }

            // Create Entries from file here
            string? sourcePath = SourcePath.NormalizePath(isLiteral: true, this);

            if (sourcePath is null)
            {
                return;
            }

            if (!sourcePath.IsArchive())
            {
                ThrowTerminatingError(
                    ExceptionHelpers.NotArchivePathError(sourcePath));
            }

            using FileStream fileStream = File.Open(
                sourcePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite);

            foreach (string entry in EntryPath)
            {
                ZipArchiveEntry? zipEntry = _zip.GetEntry(entry);

                if (zipEntry is not null)
                {
                    if (!Force.IsPresent)
                    {
                        WriteError(ExceptionHelpers.DuplicatedEntryError(
                            entry, Destination));

                        continue;
                    }

                    zipEntry.Delete();
                }

                _entries.Add(_zip.CreateEntryFromFile(
                    entry,
                    fileStream,
                    CompressionLevel));
            }
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            ThrowTerminatingError(
                ExceptionHelpers.ZipOpenError(Destination, e));
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
            _writers ??= _entries
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

            WriteObject(CreateOutput(), enumerateCollection: true);
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

    private ZipEntryBase[] CreateOutput()
    {
        using ZipArchive zip = ZipFile.OpenRead(Destination);

        List<ZipEntryBase> _result = new(_entries.Count);

        foreach (ZipArchiveEntry entry in _entries)
        {
            if (string.IsNullOrEmpty(entry.Name))
            {
                _result.Add(new ZipEntryDirectory(
                    zip.GetEntry(entry.FullName),
                    Destination));

                continue;
            }

            _result.Add(new ZipEntryFile(
                zip.GetEntry(entry.FullName),
                Destination));
        }

        return _result
            .OrderBy(SortingOps.SortByParent)
            .ThenBy(SortingOps.SortByLength)
            .ThenBy(SortingOps.SortByName)
            .ToArray();
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
