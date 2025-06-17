using System;
using System.IO;
using System.Management.Automation;
using System.Text;
using ICSharpCode.SharpZipLib.Tar;
using PSCompression.Abstractions;
using PSCompression.Exceptions;
using PSCompression.Extensions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Compress, "TarArchive")]
[OutputType(typeof(FileInfo))]
[Alias("tarcompress")]
public sealed class CompressTarArchiveCommand : ToCompressedFileCommandBase<TarOutputStream>
{
    private Stream? _compressionStream;

    [Parameter]
    [ValidateNotNull]
    public Algorithm Algorithm { get; set; } = Algorithm.gz;

    protected override string FileExtension => Algorithm is Algorithm.none ? ".tar" : $".tar.{Algorithm}";

    protected override TarOutputStream CreateCompressionStream(Stream outputStream)
    {
        _compressionStream = Algorithm.ToCompressedStream(outputStream, CompressionLevel);
        return new TarOutputStream(_compressionStream, Encoding.UTF8);
    }

    protected override void CreateDirectoryEntry(
        TarOutputStream archive,
        DirectoryInfo directory,
        string path)
    {
        archive.CreateTarEntry(
            entryName: path,
            modTime: directory.LastWriteTimeUtc,
            size: 0);

        archive.CloseEntry();
    }

    protected override void CreateOrUpdateFileEntry(
        TarOutputStream archive,
        FileInfo file,
        string path)
    {
        archive.CreateTarEntry(
            entryName: path,
            modTime: file.LastWriteTimeUtc,
            size: file.Length);

        try
        {
            using FileStream fs = OpenFileStream(file);
            fs.CopyTo(archive);
        }
        catch (Exception exception)
        {
            WriteError(exception.ToStreamOpenError(file.FullName));
        }
        finally
        {
            archive.CloseEntry();
        }
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _compressionStream?.Dispose();
    }
}
