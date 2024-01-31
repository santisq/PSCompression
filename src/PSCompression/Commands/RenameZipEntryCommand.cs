using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.Rename, "ZipEntry", SupportsShouldProcess = true)]
[OutputType(typeof(ZipEntryFile), typeof(ZipEntryDirectory))]
public sealed class RenameZipEntryCommand : PSCmdlet, IDisposable
{
    private readonly ZipArchiveCache _cache = new(ZipArchiveMode.Update);

    private ZipEntryCache? _zipEntryCache;

    private Dictionary<string, string>? _pathChanges;

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

    [Parameter]
    public SwitchParameter Recurse { get; set; }

    protected override void BeginProcessing()
    {
        if (PassThru.IsPresent)
        {
            _zipEntryCache = new();
        }

        if (Recurse.IsPresent)
        {
            _pathChanges = new(StringComparer.InvariantCultureIgnoreCase);
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
            string destination = Rename(ZipEntry);

            if (!PassThru.IsPresent || _zipEntryCache is null)
            {
                return;
            }

            _zipEntryCache.Add(
                source: ZipEntry.Source,
                path: destination,
                type: ZipEntry.Type);
        }
        catch (Exception e) when (e is PipelineStoppedException or FlowControlException)
        {
            throw;
        }
        catch (DuplicatedEntryException e)
        {
            WriteError(ExceptionHelpers.DuplicatedEntryError(e));
        }
        catch (EntryNotFoundException e)
        {
            WriteError(ExceptionHelpers.EntryNotFoundError(e));
        }
        catch (ArgumentException e)
        {
            WriteError(ExceptionHelpers.InvalidNameError(NewName, e));
        }
        catch (Exception e)
        {
            WriteError(ExceptionHelpers.WriteError(ZipEntry, e));
        }
    }

    protected override void EndProcessing()
    {
        if (_pathChanges is not null)
        {
            foreach (string path in _pathChanges.Keys)
            {

            }
        }


        _cache?.Dispose();
        if (!PassThru.IsPresent || _zipEntryCache is null)
        {
            return;
        }

        WriteObject(
            _zipEntryCache.GetEntries(),
            enumerateCollection: true);
    }

    private string Rename(ZipEntryBase entry)
    {
        if (entry is ZipEntryFile file)
        {
            return file.Rename(
                newname: NewName,
                zip: _cache.GetOrAdd(file));
        }

        string destination = ((ZipEntryDirectory)entry).Rename(
            newname: NewName,
            zip: _cache.GetOrAdd(entry));

        if (_pathChanges is not null)
        {
            _pathChanges[destination] = ZipEntry.RelativePath;
        }

        return destination;
    }

    public void Dispose() => _cache?.Dispose();
}
