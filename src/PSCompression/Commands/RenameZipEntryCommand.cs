using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Exceptions;
using PSCompression.Extensions;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.Rename, "ZipEntry", SupportsShouldProcess = true)]
[OutputType(typeof(ZipEntryFile), typeof(ZipEntryDirectory))]
public sealed class RenameZipEntryCommand : PSCmdlet, IDisposable
{
    private readonly ZipArchiveCache _zipArchiveCache = new(ZipArchiveMode.Update);

    private ZipEntryCache? _zipEntryCache;

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
        if (PassThru.IsPresent)
        {
            _zipEntryCache = new();
        }
    }

    protected override void ProcessRecord()
    {
        if (!ShouldProcess(target: ZipEntry.ToString(), action: "Rename"))
        {
            return;
        }

        try
        {
            ZipEntry.ThrowIfFromStream();
            NewName.ThrowIfInvalidNameChar();
            _zipArchiveCache.TryAdd(ZipEntry);
            _moveCache.AddEntry(ZipEntry, NewName);
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
        foreach (var mapping in _moveCache.GetMappings(_zipArchiveCache))
        {
            Rename(mapping);
        }

        _zipArchiveCache?.Dispose();
        if (!PassThru.IsPresent || _zipEntryCache is null)
        {
            return;
        }

        WriteObject(
            _zipEntryCache
                .AddRange(_moveCache.GetPassThruMappings())
                .GetEntries()
                .ZipEntrySort(),
            enumerateCollection: true);
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
                    _zipArchiveCache[mapping.Key]);
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
        _zipArchiveCache?.Dispose();
        GC.SuppressFinalize(this);
    }
}
