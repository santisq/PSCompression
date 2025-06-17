using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Extensions;
using PSCompression.Exceptions;
using PSCompression.Abstractions;

namespace PSCompression.Commands;

[Cmdlet(VerbsData.Compress, "ZipArchive")]
[OutputType(typeof(FileInfo))]
[Alias("zipcompress")]
public sealed class CompressZipArchiveCommand : ToCompressedFileCommandBase<ZipArchive>
{
    private ZipArchiveMode ZipArchiveMode
    {
        get => Force.IsPresent || Update.IsPresent
            ? ZipArchiveMode.Update
            : ZipArchiveMode.Create;
    }

    protected override string FileExtension => ".zip";

    private void CreateEntry(
        FileInfo file,
        ZipArchive zip,
        string relativepath)
    {
        try
        {
            using FileStream fileStream = OpenFileStream(file);
            using Stream stream = zip
                .CreateEntry(relativepath, CompressionLevel)
                .Open();

            fileStream.CopyTo(stream);
        }
        catch (Exception exception)
        {
            WriteError(exception.ToStreamOpenError(file.FullName));
        }
    }

    private void UpdateEntry(
        FileInfo file,
        ZipArchiveEntry entry)
    {
        try
        {
            using FileStream fileStream = OpenFileStream(file);
            using Stream stream = entry.Open();
            stream.SetLength(0);
            fileStream.CopyTo(stream);
        }
        catch (Exception exception)
        {
            WriteError(exception.ToStreamOpenError(file.FullName));
        }
    }

    protected override ZipArchive CreateCompressionStream(Stream outputStream) =>
        new(outputStream, ZipArchiveMode);

    protected override void CreateDirectoryEntry(
        ZipArchive archive,
        DirectoryInfo directory,
        string path)
    {
        if (!Update || !archive.TryGetEntry(path, out _))
        {
            archive.CreateEntry(path);
        }
    }

    protected override void CreateOrUpdateFileEntry(
        ZipArchive archive,
        FileInfo file,
        string path)
    {
        if (Update && archive.TryGetEntry(path, out ZipArchiveEntry? entry))
        {
            UpdateEntry(file, entry);
            return;
        }

        CreateEntry(file, archive, path);
    }
}
