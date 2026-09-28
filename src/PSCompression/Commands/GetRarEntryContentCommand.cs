using System;
using System.IO;
using System.Management.Automation;
using System.Security;
using PSCompression.Abstractions.Commands;
using PSCompression.Extensions;
using PSCompression.FormatHandlers.Common;
using PSCompression.FormatHandlers.Rar;
using SharpCompress.Archives;
using SharpCompress.Archives.Rar;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.Get, "RarEntryContent", DefaultParameterSetName = "Stream")]
[OutputType(typeof(string), ParameterSetName = ["Stream"])]
[OutputType(typeof(byte), ParameterSetName = ["Bytes"])]
[Alias("rargec")]
public sealed class GetRarEntryContentCommand : GetEntryContentCommandBase<RarEntry>, IDisposable
{
    private readonly ArchiveCache<IRarArchive, RarEntry> _cache = new(e => e.OpenRead());

    [Parameter]
    public SecureString? Password { get; set; }

    protected override void ProcessRecord()
    {
        foreach (RarEntry entry in Entry)
        {
            if (entry.Type == Enum.EntryType.Directory || entry.Length == 0)
                continue;

            try
            {
                IRarArchive rar = _cache.GetOrCreate(entry);

                if (entry.IsEncrypted)
                {
                    rar.ReaderOptions.Password = Password is null
                        ? entry.PromptForPassword(Host)
                        : Password.AsPlainText();
                }

                ReadEntry(rar.GetEntry(entry));
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

    private void ReadEntry(IArchiveEntry entry)
    {
        using Stream stream = entry.OpenEntryStream();

        if (AsByteStream)
        {
            Dbg.Assert(Buffer is not null);
            using EntryByteReader byteReader = new(stream, Buffer);

            if (Raw)
            {
                byteReader.ReadAllBytes(this);
                return;
            }

            byteReader.StreamBytes(this);
            return;
        }

        using StreamReader reader = new(stream, Encoding);
        if (Raw)
        {
            reader.ReadToEnd(this);
            return;
        }

        reader.ReadLines(this);
    }

    public void Dispose() => _cache.Dispose();
}
