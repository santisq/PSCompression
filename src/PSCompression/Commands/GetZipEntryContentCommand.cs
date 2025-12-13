using System;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Abstractions;
using PSCompression.Exceptions;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.Get, "ZipEntryContent", DefaultParameterSetName = "Stream")]
[OutputType(typeof(string), ParameterSetName = ["Stream"])]
[OutputType(typeof(byte), ParameterSetName = ["Bytes"])]
[Alias("zipgec")]
public sealed class GetZipEntryContentCommand : GetEntryContentCommandBase<ZipEntryFile>, IDisposable
{
    private readonly ZipArchiveCache<ZipArchive> _cache = new(entry => entry.OpenRead());

    protected override void ProcessRecord()
    {
        foreach (ZipEntryFile entry in Entry)
        {
            try
            {
                ZipContentReader reader = new(_cache.GetOrCreate(entry));
                ReadEntry(entry, reader);
            }
            catch (Exception _) when (_ is PipelineStoppedException or FlowControlException)
            {
                throw;
            }
            catch (Exception exception)
            {
                WriteError(exception.ToOpenError(entry.Source));
            }
        }
    }

    private void ReadEntry(ZipEntryFile entry, ZipContentReader reader)
    {
        if (AsByteStream)
        {
            if (Raw)
            {
                reader.ReadAllBytes(entry, this);
                return;
            }

            reader.StreamBytes(entry, BufferSize, this);
            return;
        }

        if (Raw.IsPresent)
        {
            reader.ReadToEnd(entry, Encoding, this);
            return;
        }

        reader.StreamLines(entry, Encoding, this);
    }

    public void Dispose()
    {
        _cache?.Dispose();
        GC.SuppressFinalize(this);
    }
}
