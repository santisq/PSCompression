using System;
using System.IO;
using System.Management.Automation;
using PSCompression.Abstractions;
using PSCompression.Exceptions;
using PSCompression.Extensions;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.Get, "TarEntryContent", DefaultParameterSetName = "Stream")]
[OutputType(typeof(string), ParameterSetName = ["Stream"])]
[OutputType(typeof(byte), ParameterSetName = ["Bytes"])]
[Alias("targec")]
public sealed class GetTarEntryContentCommand : GetEntryContentCommandBase<TarEntryFile>
{
    protected override void ProcessRecord()
    {
        foreach (TarEntryFile entry in Entry)
        {
            try
            {
                ReadEntry(entry);
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

    private void ReadEntry(TarEntryFile entry)
    {
        using MemoryStream mem = new();
        if (!entry.GetContentStream(mem))
        {
            return;
        }

        if (AsByteStream)
        {
            if (Raw)
            {
                WriteObject(mem.ToArray());
                return;
            }

            using EntryByteReader byteReader = new(mem, Buffer!);
            byteReader.StreamBytes(this);
            return;
        }

        using StreamReader reader = new(mem, Encoding);
        if (Raw)
        {
            reader.ReadToEnd(this);
            return;
        }

        reader.ReadLines(this);
    }
}
