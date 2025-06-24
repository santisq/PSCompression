using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text;
using ICSharpCode.SharpZipLib.Tar;
using PSCompression.Abstractions;
using PSCompression.Extensions;

namespace PSCompression.Commands;

[Cmdlet(VerbsCommon.Get, "TarEntry", DefaultParameterSetName = "Path")]
[OutputType(typeof(TarEntryDirectory), typeof(TarEntryFile))]
[Alias("targe")]
public sealed class GetTarEntryCommand : GetEntryCommandBase
{
    [Parameter]
    public Algorithm Algorithm { get; set; }

    internal override ArchiveType ArchiveType => ArchiveType.tar;

    protected override IEnumerable<EntryBase> GetEntriesFromFile(string path)
    {
        if (!MyInvocation.BoundParameters.ContainsKey(nameof(Algorithm)))
        {
            Algorithm = AlgorithmMappings.Parse(path);
        }

        using FileStream fs = File.OpenRead(path);
        using Stream stream = Algorithm.FromCompressedStream(fs);
        using TarInputStream tar = new(stream, Encoding.UTF8);

        foreach (TarEntry entry in tar.EnumerateEntries())
        {
            if (ShouldSkipEntry(entry.IsDirectory))
            {
                continue;
            }

            if (!ShouldInclude(entry.Name) || ShouldExclude(entry.Name))
            {
                continue;
            }

            yield return entry.IsDirectory
                ? new TarEntryDirectory(entry, path)
                : new TarEntryFile(entry, path, Algorithm);
        }
    }

    protected override IEnumerable<EntryBase> GetEntriesFromStream(Stream stream)
    {
        Stream decompressStream = Algorithm.FromCompressedStream(stream);
        TarInputStream tar = new(decompressStream, Encoding.UTF8);

        foreach (TarEntry entry in tar.EnumerateEntries())
        {
            if (ShouldSkipEntry(entry.IsDirectory))
            {
                continue;
            }

            if (!ShouldInclude(entry.Name) || ShouldExclude(entry.Name))
            {
                continue;
            }

            yield return entry.IsDirectory
                ? new TarEntryDirectory(entry, stream)
                : new TarEntryFile(entry, stream, Algorithm);
        }
    }
}
