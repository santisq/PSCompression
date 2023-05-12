using System.IO.Compression;
using System.Linq;
using System.Management.Automation;
using System.Text.RegularExpressions;

namespace PSCompression;

[Cmdlet(VerbsCommon.New, "ZipEntry")]
[OutputType(typeof(ZipEntryDirectory), typeof(ZipEntryDirectory))]
public sealed class NewZipEntryCommand : CommandsBase
{
    private Regex _re = new(@"[\\/]$", RegexOptions.Compiled);

    [Parameter(
        Mandatory = true,
        ValueFromPipelineByPropertyName = true,
        Position = 0
    )]
    [Alias("PSPath")]
    public string LiteralPath { get; set; } = null!;

    [Parameter(Mandatory = true, Position = 1)]
    public string[] EntryRelativePath { get; set; } = null!;

    [Parameter(Position = 2)]
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

        foreach (string entryPath in EntryRelativePath)
        {
            if(_re.IsMatch(entryPath))
            {
                new ZipEntryDirectory(CreateEntry(entryPath, path), path)
            }
        }
    }

    private ZipArchiveEntry CreateEntry(string entryPath, string source)
    {
        using ZipArchive zip = ZipFile.Open(source, ZipArchiveMode.Update);
        return zip.CreateEntry(entryPath, CompressionLevel);
    }

}
