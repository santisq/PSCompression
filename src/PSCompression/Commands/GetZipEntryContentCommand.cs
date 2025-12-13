using System;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Security;
using ICSharpCode.SharpZipLib.Zip;
using PSCompression.Abstractions;
using PSCompression.Exceptions;
using PSCompression.Extensions;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.Get, "ZipEntryContent", DefaultParameterSetName = "Stream")]
[OutputType(typeof(string), ParameterSetName = ["Stream"])]
[OutputType(typeof(byte), ParameterSetName = ["Bytes"])]
[Alias("zipgec")]
public sealed class GetZipEntryContentCommand : GetEntryContentCommandBase<ZipEntryFile>, IDisposable
{
    [Parameter]
    public SecureString? Password { get; set; }

    private ZipArchiveCache<ZipFile>? _cache;

    protected override void ProcessRecord()
    {
        ZipFile zip;
        _cache ??= new ZipArchiveCache<ZipFile>(entry => entry.OpenRead(Password));

        foreach (ZipEntryFile entry in Entry)
        {
            try
            {
                zip = _cache.GetOrCreate(entry);
                if (entry.IsEncrypted && Password is null)
                {
                    entry.PromptForCredential(zip, Host);
                }

                ZipContentReader reader = new(zip);
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
