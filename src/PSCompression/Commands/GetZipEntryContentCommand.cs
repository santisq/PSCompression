using System;
using System.IO;
using System.Management.Automation;
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
        _cache ??= new ZipArchiveCache<ZipFile>(entry => entry.OpenRead(Password));

        foreach (ZipEntryFile entry in Entry)
        {
            try
            {
                ZipFile zip = _cache.GetOrCreate(entry);
                if (entry.IsEncrypted && Password is null)
                {
                    zip.Password = entry.PromptForPassword(Host);
                }

                ReadEntry(entry.Open(zip));
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

    private void ReadEntry(Stream stream)
    {
        if (AsByteStream)
        {
            using EntryByteReader byteReader = new(stream, Buffer!);
            if (Raw)
            {
                byteReader.ReadAllBytes(this);
                return;
            }

            byteReader.StreamBytes(this);
            return;
        }

        using StreamReader stringReader = new(stream, Encoding);
        if (Raw)
        {
            stringReader.ReadToEnd(this);
            return;
        }

        stringReader.ReadLines(this);
    }

    public void Dispose()
    {
        _cache?.Dispose();
        GC.SuppressFinalize(this);
    }
}
