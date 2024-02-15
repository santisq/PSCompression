using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Exceptions;
using static PSCompression.Exceptions.ExceptionHelpers;

namespace PSCompression;

[Cmdlet(VerbsCommon.Rename, "ZipEntry", SupportsShouldProcess = true)]
[OutputType(typeof(ZipEntryFile), typeof(ZipEntryDirectory))]
public sealed class RenameZipEntryCommand : PSCmdlet, IDisposable
{
    private readonly ZipArchiveCache _zipArchiveCache = new(ZipArchiveMode.Update);

    private ZipEntryCache? _zipEntryCache;

    private readonly Dictionary<string, (ZipEntryBase, string)> _pathChanges =
        new(StringComparer.InvariantCultureIgnoreCase);

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
            // string destination = Rename(ZipEntry);
            _zipArchiveCache.TryAdd(ZipEntry);

            _moveCache.AddEntry(ZipEntry, NewName);

            _pathChanges[ZipEntry.RelativePath] = (ZipEntry, NewName);

            if (!PassThru.IsPresent || _zipEntryCache is null)
            {
                return;
            }

            // _zipEntryCache.Add(
            //     source: ZipEntry.Source,
            //     path: destination,
            //     type: ZipEntry.Type);

        }
        catch (Exception e) when (e is PipelineStoppedException or FlowControlException)
        {
            throw;
        }
        catch (DuplicatedEntryException e)
        {
            WriteError(DuplicatedEntryError(e));
        }
        catch (EntryNotFoundException e)
        {
            WriteError(EntryNotFoundError(e));
        }
        catch (ArgumentException e)
        {
            WriteError(InvalidNameError(NewName, e));
        }
        catch (Exception e)
        {
            WriteError(ZipWriteError(ZipEntry, e));
        }
    }

    protected override void EndProcessing()
    {
        WriteObject(_moveCache.GetMappings(_zipArchiveCache));
        return;

        // _zipArchiveCache?.Dispose();
        // if (!PassThru.IsPresent || _zipEntryCache is null)
        // {
        //     return;
        // }

        // WriteObject(
        //     _zipEntryCache.GetEntries(),
        //     enumerateCollection: true);
    }

    // private Dictionary<string, (string, string)> NewMethod()
    // {
    //     Dictionary<string, (string, string)> _pathResults =
    //         new(StringComparer.InvariantCultureIgnoreCase);

    //     string newpath;
    //     foreach (var pair in _pathChanges.OrderByDescending(e => e.Key))
    //     {
    //         (ZipEntryBase entry, string newname) = pair.Value;
    //         if (entry.Type is ZipEntryType.Archive)
    //         {
    //             newpath = ((ZipEntryFile)entry).ChangeName(newname);
    //             _pathResults[pair.Key] = (entry.Source, newpath);
    //             continue;
    //         }

    //         ZipEntryDirectory dir = (ZipEntryDirectory)entry;
    //         newpath = dir.ChangeName(newname);
    //         _pathResults[pair.Key] = (entry.Source, newpath);
    //         Regex re = new(
    //             Regex.Escape(dir.RelativePath),
    //             RegexOptions.Compiled | RegexOptions.IgnoreCase);

    //         foreach (ZipArchiveEntry key in dir.GetChilds(_zipArchiveCache[dir.Source]))
    //         {
    //             (string source, string child) = _pathResults.ContainsKey(key.FullName)
    //                 ? _pathResults[key.FullName]
    //                 : (dir.Source, key.FullName);

    //             _pathResults[key.FullName] = (source, re.Replace(child, newpath));
    //         }
    //     }

    //     return _pathResults;
    // }

    // private string Rename(ZipEntryBase entry)
    // {
    //     if (entry is ZipEntryFile file)
    //     {
    //         return file.Rename(
    //             newname: NewName,
    //             zip: _zipArchiveCache.GetOrAdd(file));
    //     }

    //     string destination = ((ZipEntryDirectory)entry).Rename(
    //         newname: NewName,
    //         zip: _zipArchiveCache.GetOrAdd(entry));

    //     if (_pathChanges is not null)
    //     {
    //         _pathChanges[destination] = ZipEntry.RelativePath;
    //     }

    //     return destination;
    // }

    public void Dispose() => _zipArchiveCache?.Dispose();
}
