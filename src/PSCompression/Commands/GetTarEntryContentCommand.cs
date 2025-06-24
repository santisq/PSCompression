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
    private byte[]? _buffer;

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

            StreamBytes(mem);
            return;
        }

        using StreamReader reader = new(mem, Encoding);

        if (Raw.IsPresent)
        {
            reader.WriteAllTextToPipeline(this);
            return;
        }

        reader.WriteLinesToPipeline(this);
    }

    private void StreamBytes(Stream stream)
    {
        int bytesRead;
        _buffer ??= new byte[BufferSize];

        while ((bytesRead = stream.Read(_buffer, 0, BufferSize)) > 0)
        {
            for (int i = 0; i < bytesRead; i++)
            {
                WriteObject(_buffer[i]);
            }
        }
    }
}
