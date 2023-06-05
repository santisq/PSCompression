using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;

namespace PSCompression;

[Cmdlet(VerbsCommon.New, "ZipEntry")]
[OutputType(typeof(ZipEntryDirectory), typeof(ZipEntryFile))]
public sealed class NewZipEntryCommand : PSCompressionCommandsBase
{
    private readonly List<ZipEntryBase> _result = new();

    [Parameter(Mandatory = true, Position = 0)]
    public string LiteralPath { get; set; } = null!;

    [Parameter(Mandatory = true, Position = 1)]
    public string[] EntryRelativePath { get; set; } = null!;

    [Parameter]
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    protected override void ProcessRecord()
    {
        (string path, ProviderInfo provider) = NormalizePaths(
            new string[1] { LiteralPath },
            isLiteral: true).FirstOrDefault();

        if (!ValidatePath(path, provider))
        {
            return;
        }

        try
        {
            WriteObject(CreateEntries(path), enumerateCollection: true);
        }
        catch (PipelineStoppedException)
        {
            throw;
        }
        catch (Exception e)
        {
            WriteError(new ErrorRecord(
                e, "ZipOpen", ErrorCategory.OpenError, path));
        }
    }

    private ZipEntryBase[] CreateEntries(string path)
    {
        using ZipArchive zip = ZipFile.Open(path, ZipArchiveMode.Update);
        _result.Clear();

        foreach (string entryPath in EntryRelativePath)
        {
            if (entryPath.IsDirectoryPath())
            {
                _result.Add(new ZipEntryDirectory(
                    zip.CreateEntry(entryPath.ToNormalizedEntryPath(), CompressionLevel), path));

                continue;
            }

            _result.Add(new ZipEntryFile(
                zip.CreateEntry(entryPath.ToNormalizedFileEntryPath(), CompressionLevel), path));
        }

        return _result.ToArray();
    }
}
