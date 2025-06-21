using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Runtime.CompilerServices;
using System.Text;
using ICSharpCode.SharpZipLib.Tar;
using PSCompression.Abstractions;
using PSCompression.Exceptions;
using PSCompression.Extensions;
using IO = System.IO;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Expand, "TarArchive")]
[OutputType(typeof(FileInfo), typeof(DirectoryInfo))]
public sealed class ExpandTarArchiveCommand : CommandWithPathBase
{
    private bool _shouldInferAlgo;

    [Parameter(Position = 1)]
    public string? Destination { get; set; }

    [Parameter]
    public Algorithm Algorithm { get; set; }

    [Parameter]
    public SwitchParameter Force { get; set; }

    [Parameter]
    public SwitchParameter PassThru { get; set; }

    protected override void BeginProcessing()
    {
        Destination = Destination is null
            // PowerShell is retarded and decided to mix up ProviderPath & Path
            ? SessionState.Path.CurrentFileSystemLocation.ProviderPath
            : Destination.ResolvePath(this);

        if (File.Exists(Destination))
        {
            ThrowTerminatingError(ExceptionHelper.NotDirectoryPath(
                Destination, nameof(Destination)));
        }

        Directory.CreateDirectory(Destination);

        _shouldInferAlgo = !MyInvocation.BoundParameters
            .ContainsKey(nameof(Algorithm));
    }

    protected override void ProcessRecord()
    {
        Dbg.Assert(Destination is not null);

        foreach (string path in EnumerateResolvedPaths())
        {
            if (_shouldInferAlgo)
            {
                Algorithm = AlgorithmMappings.Parse(path);
            }

            try
            {
                FileSystemInfo[] output = ExtractArchive(path);

                if (PassThru)
                {
                    IOrderedEnumerable<PSObject> result = output
                        .Select(AppendPSProperties)
                        .OrderBy(pso => pso.Properties["PSParentPath"].Value);

                    WriteObject(result, enumerateCollection: true);
                }
            }
            catch (Exception _) when (_ is PipelineStoppedException or FlowControlException)
            {
                throw;
            }
            catch (Exception exception)
            {
                WriteError(exception.ToWriteError(path));
            }
        }
    }

    private FileSystemInfo[] ExtractArchive(string path)
    {
        using FileStream fs = File.OpenRead(path);
        using Stream decompress = Algorithm.FromCompressedStream(fs);
        using TarInputStream tar = new(decompress, Encoding.UTF8);

        List<FileSystemInfo> result = [];
        foreach (TarEntry entry in tar.EnumerateEntries())
        {
            try
            {
                result.Add(ExtractEntry(entry, tar));
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
        string destination = IO.Path.GetFullPath(
            IO.Path.Combine(Destination, entry.Name));

        if (entry.IsDirectory)
        {
            DirectoryInfo dir = new(destination);
            dir.Create(Force);
            return dir;
        }

        FileInfo file = new(destination);
        file.Directory.Create();

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

    private static PSObject AppendPSProperties(FileSystemInfo info)
    {
        string parent = info is DirectoryInfo dir
            ? dir.Parent.FullName
            : Unsafe.As<FileInfo>(info).DirectoryName;

        return info.AppendPSProperties(parent);
    }
}
