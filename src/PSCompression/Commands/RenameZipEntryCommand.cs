using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Abstractions.Entries;
using PSCompression.Exceptions;
using PSCompression.Extensions;
using PSCompression.FormatHandlers.Common;
using PSCompression.FormatHandlers.Zip;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.Rename, "ZipEntry", SupportsShouldProcess = true)]
[OutputType(typeof(ZipEntryFile), typeof(ZipEntryDirectory))]
[Alias("zipren")]
public sealed class RenameZipEntryCommand : PSCmdlet, IDisposable
{
    private readonly ArchiveCache<ZipArchive, ZipEntryBase> _archiveCache = new(
        entry => entry.OpenZip(ZipArchiveMode.Update));

    private ZipEntryCache? _entryCache;

    private readonly ZipEntryMoveCache _moveCache = new();

    [Parameter(
        Mandatory = true,
        Position = 0,
        ValueFromPipeline = true)]
    public ZipEntryBase ZipEntry { get; set; } = null!;

    [Parameter(
        Mandatory = true,
        Position = 1,
        ValueFromPipeline = true)]
    public string NewName { get; set; } = null!;

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing()
    {
        if (PassThru) _entryCache = new();
    }

    protected override void ProcessRecord()
    {
        if (!ShouldProcess(target: ZipEntry.ToString(), action: "Rename"))
            return;

        try
        {
            ZipEntry.ThrowIfFromStream();
            NewName.ThrowIfInvalidNameChar();
            _archiveCache.TryAdd(ZipEntry);
            _moveCache.AddEntry(ZipEntry, NewName);
        }
        catch (NotSupportedException exception)
        {
            ThrowTerminatingError(exception.ToStreamOpenError(ZipEntry));
        }
        catch (InvalidNameException exception)
        {
            WriteError(exception.ToInvalidNameError(NewName));
        }
        catch (Exception exception)
        {
            WriteError(exception.ToOpenError(ZipEntry.Source));
        }
    }

    protected override void EndProcessing()
    {
        Dictionary<string, Dictionary<string, string>> mappings = _moveCache.GetMappings(_archiveCache);
        foreach (KeyValuePair<string, Dictionary<string, string>> mapping in mappings)
            Rename(mapping);

        _archiveCache.Dispose();
        if (!PassThru || _entryCache is null) return;

        IEnumerable<EntryBase> entries = _entryCache
            .AddRange(_moveCache.GetPassThruMappings())
            .GetEntries()
            .SortEntries();

        foreach (EntryBase entry in entries) WriteObject(entry);
    }

    private void Rename(KeyValuePair<string, Dictionary<string, string>> mapping)
    {
        foreach ((string source, string destination) in mapping.Value)
        {
            try
            {
                ZipEntryBase.Move(
                    sourceRelativePath: source,
                    destination: destination,
                    sourceZipPath: mapping.Key,
                    _archiveCache[mapping.Key]);
            }
            catch (DuplicatedEntryException exception)
            {
                if (_moveCache.IsDirectoryEntry(mapping.Key, source))
                {
                    ThrowTerminatingError(exception.ToDuplicatedEntryError());
                }

                WriteError(exception.ToDuplicatedEntryError());
            }
            catch (EntryNotFoundException exception)
            {
                WriteError(exception.ToEntryNotFoundError());
            }
            catch (Exception exception)
            {
                WriteError(exception.ToWriteError(ZipEntry));
            }
        }
    }

    public void Dispose()
    {
        _archiveCache.Dispose();
    }
}
