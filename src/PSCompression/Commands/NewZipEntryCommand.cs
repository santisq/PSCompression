using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using System.Text;
using PSCompression.Extensions;
using PSCompression.Exceptions;
using PSCompression.Abstractions;
using ICSharpCode.SharpZipLib.Zip;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.New, "ZipEntry", DefaultParameterSetName = "Value")]
[OutputType(typeof(ZipEntryDirectory), typeof(ZipEntryFile))]
[Alias("zipne")]
public sealed class NewZipEntryCommand : PSCmdlet, IDisposable
{
    private readonly List<ZipArchiveEntry> _entries = [];

    private ZipArchive? _zip;

    private ZipContentWriter[]? _writers;

    private string[]? _entryPath;

    [Parameter(ValueFromPipeline = true, ParameterSetName = "Value")]
    [ValidateNotNull]
    public string[]? Value { get; set; }

    [Parameter(Mandatory = true, Position = 0)]
    public string Destination { get; set; } = null!;

    [Parameter(ParameterSetName = "File", Position = 1)]
    [ValidateNotNullOrEmpty]
    public string? SourcePath { get; set; }

    [Parameter(ParameterSetName = "Value", Mandatory = true, Position = 2)]
    [Parameter(ParameterSetName = "File", Position = 2)]
    public string[]? EntryPath
    {
        get => _entryPath;
        set => _entryPath = [.. value!.Select(e => e.NormalizePath())];
    }

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
        Destination = Destination.ResolvePath(this);
        Destination.WriteErrorIfNotArchive(nameof(Destination), this, isTerminating: true);

        try
        {
            _zip = System.IO.Compression.ZipFile.Open(Destination, ZipArchiveMode.Update);

            if (ParameterSetName == "Value")
            {
                // Mandatory Parameter on this ParameterSet
                Dbg.Assert(EntryPath is not null);
                // We can create the entries here and go the process block
                foreach (string entry in EntryPath)
                {
                    if (_zip.TryGetEntry(entry, out ZipArchiveEntry? zipentry))
                    {
                        if (!Force.IsPresent)
                        {
                            WriteError(DuplicatedEntryException
                                .Create(entry, Destination)
                                .ToDuplicatedEntryError());

                            continue;
                        }

                        zipentry.Delete();
                    }

                    _entries.Add(_zip.CreateEntry(entry, CompressionLevel));
                }

                return;
            }

            // else, we're on File ParameterSet, this can't be null
            Dbg.Assert(SourcePath is not null);
            // Create Entries from file here
            SourcePath = SourcePath.ResolvePath(this);
            SourcePath.WriteErrorIfNotArchive(nameof(SourcePath), this, isTerminating: true);

            using FileStream fileStream = File.Open(
                path: SourcePath,
                mode: FileMode.Open,
                access: FileAccess.Read,
                share: FileShare.ReadWrite);

            EntryPath ??= [SourcePath.NormalizePath()];
            foreach (string entry in EntryPath)
            {
                if (_zip.TryGetEntry(entry, out ZipArchiveEntry? zipentry))
                {
                    if (!Force.IsPresent)
                    {
                        WriteError(DuplicatedEntryException
                            .Create(entry, Destination)
                            .ToDuplicatedEntryError());

                        continue;
                    }

                    zipentry.Delete();
                }

                _entries.Add(_zip.CreateEntryFromFile(
                    entry: entry,
                    fileStream: fileStream,
                    compressionLevel: CompressionLevel));
            }
        }
        catch (Exception _) when (_ is PipelineStoppedException or FlowControlException)
        {
            throw;
        }
        catch (Exception exception)
        {
            ThrowTerminatingError(exception.ToOpenError(Destination));
        }
    }

    protected override void ProcessRecord()
    {
        Dbg.Assert(_zip is not null);

        // no input from pipeline, go to end block
        if (Value is null)
        {
            return;
        }

        try
        {
            _writers ??= [.. _entries
                .Where(e => !string.IsNullOrEmpty(e.Name))
                .Select(e => new ZipContentWriter(_zip, e, Encoding))];

            foreach (ZipContentWriter writer in _writers)
            {
                writer.WriteLines(Value);
            }
        }
        catch (Exception exception)
        {
            ThrowTerminatingError(exception.ToWriteError(_zip));
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
            GetResult();
        }
        catch (Exception _) when (_ is PipelineStoppedException or FlowControlException)
        {
            throw;
        }
        catch (Exception exception)
        {
            ThrowTerminatingError(exception.ToOpenError(Destination));
        }
    }

    private void GetResult()
    {
        using ICSharpCode.SharpZipLib.Zip.ZipFile zip = new(Destination);
        List<EntryBase> _result = new(_entries.Count);

        foreach (ZipArchiveEntry entry in _entries)
        {
            if (!zip.TryGetEntry(entry.FullName, out ZipEntry? zipEntry))
            {
                continue;
            }

            _result.Add(zipEntry.IsDirectory
                ? new ZipEntryDirectory(zipEntry, Destination)
                : new ZipEntryFile(zipEntry, Destination));
        }

        WriteObject(_result.ToEntrySort(), enumerateCollection: true);
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
        GC.SuppressFinalize(this);
    }
}
