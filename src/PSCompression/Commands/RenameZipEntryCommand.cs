using System;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Exceptions;
using PSCompression.Extensions;
using static PSCompression.Exceptions.ExceptionHelpers;

namespace PSCompression;

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
            _zipArchiveCache.TryAdd(ZipEntry);
            _moveCache.AddEntry(ZipEntry, NewName);

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
        catch (Exception e)
        {
            WriteError(ZipOpenError(ZipEntry.Source, e));
        }
    }

    protected override void EndProcessing()
    {
        foreach (var mapping in _moveCache.GetMappings(_zipArchiveCache))
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
        }

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

    public void Dispose() => _zipArchiveCache?.Dispose();
}
