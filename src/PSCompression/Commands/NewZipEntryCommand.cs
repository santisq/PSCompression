using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace PSCompression;

[Cmdlet(VerbsCommon.New, "ZipFileEntry")]
[OutputType(typeof(ZipEntryDirectory), typeof(ZipEntryDirectory))]
public sealed class NewZipEntryCommand : CommandsBase
{
    private readonly Regex _re = new(@"[\\/]$", RegexOptions.Compiled);

    private readonly List<ZipEntryBase> _result = new();

    [Parameter(Mandatory = true, Position = 0)]
    public string LiteralPath { get; set; } = null!;

    [Parameter(Mandatory = true)]
    public string[] EntryRelativePath { get; set; } = null!;

    [Parameter(Position = 1)]
    public object[]? Value { get; set; }

    [Parameter]
    public CompressionLevel CompressionLevel { get; set; } = CompressionLevel.Optimal;

    protected override void ProcessRecord()
    {
        (string path, ProviderInfo provider) = NormalizePaths(
            new string[1] { LiteralPath }, true).FirstOrDefault();

        if (!ValidatePath(path, provider))
        {
            return;
        }

        try
        {
            using (ZipArchive zip = ZipFile.Open(path, ZipArchiveMode.Update))
            {
                _result.Clear();

                foreach (string entryPath in EntryRelativePath)
                {
                    ZipArchiveEntry entry = zip.CreateEntry(entryPath, CompressionLevel);

                    if (_re.IsMatch(entryPath))
                    {
                        _result.Add(new ZipEntryDirectory(entry, path));
                        continue;
                    }

                    _result.Add(new ZipEntryFile(entry, path));
                }
            }

            WriteObject(_result.ToArray(), enumerateCollection: true);
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
}
