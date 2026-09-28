using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text;
using ICSharpCode.SharpZipLib.Tar;
using PSCompression.Abstractions.Commands;
using PSCompression.Enum;
using PSCompression.Extensions;
using PSCompression.FormatHandlers.Tar;
using IO = System.IO;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "TarArchive")]
[OutputType(typeof(FileInfo), typeof(DirectoryInfo))]
[Alias("untar")]
public sealed class ExpandTarArchiveCommand : ExpandArchiveCommandBase
{
    private bool _shouldInferAlgo;

    [Parameter]
    public Algorithm Algorithm { get; set; }

    protected override void BeginProcessing()
    {
        base.BeginProcessing();
        _shouldInferAlgo = !MyInvocation.HasBound(nameof(Algorithm));
    }

    protected override PSObject[] ExtractArchive(string source, string destination)
    {
        if (_shouldInferAlgo) Algorithm = AlgorithmMappings.Parse(source);

        using FileStream fs = File.OpenRead(source);
        using Stream decompress = Algorithm.FromCompressedStream(fs);
        using TarInputStream tar = new(decompress, Encoding.UTF8);

        List<PSObject> result = [];
        foreach (TarEntry entry in tar.EnumerateEntries())
        {
            try
            {
                FileSystemInfo info = ExtractEntry(entry, tar);
                if (PassThru) result.Add(info.AppendPSProperties());
            }
            catch (Exception _) when (_ is PipelineStoppedException or FlowControlException)
            {
                throw;
            }
            catch (Exception exception)
            {
                WriteError(exception.ToExtractEntryError(entry));
            }
        }

        return [.. result];
    }

    private FileSystemInfo ExtractEntry(TarEntry entry, TarInputStream tar)
    {
        Dbg.Assert(Destination is not null);

        string destination = IO.Path.GetFullPath(
            IO.Path.Combine(Destination, entry.Name));

        if (entry.IsDirectory)
        {
            DirectoryInfo dir = new(destination);
            dir.Create();
            return dir;
        }

        FileInfo file = new(destination);
        file.Directory?.Create();

        using (FileStream destStream = File.Open(
            destination,
            Force ? FileMode.Create : FileMode.CreateNew,
            FileAccess.Write))
        {
            if (entry.Size > 0)
            {
                tar.CopyTo(destStream, (int)entry.Size);
            }
        }

        return file;
    }
}
