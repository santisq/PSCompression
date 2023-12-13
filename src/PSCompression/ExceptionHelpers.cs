using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;

namespace PSCompression;

internal static class ExceptionHelpers
{
    private static readonly char[] s_InvalidFileNameChar = Path.GetInvalidFileNameChars();

    private static readonly char[] s_InvalidPathChar = Path.GetInvalidPathChars();

    internal static ErrorRecord NotArchivePathError(string path, string paramname) =>
        new(new ArgumentException($"The specified path is a Directory: '{path}'.", paramname),
            "NotArchivePath", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord NotDirectoryPathError(string path, string paramname) =>
        new(new ArgumentException($"Destination path is an existing File: '{path}'.", paramname),
            "NotDirectoryPath", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord InvalidProviderError(string path, ProviderInfo provider) =>
        new(new ArgumentException($"The resolved path '{path}' is not a FileSystem path but '{provider.Name}'."),
            "NotFileSystemPath", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord ZipOpenError(string path, Exception exception) =>
        new(exception, "ZipOpen", ErrorCategory.OpenError, path);

    internal static ErrorRecord ResolvePathError(string path, Exception exception) =>
        new(exception, "ResolvePath", ErrorCategory.NotSpecified, path);

    internal static ErrorRecord ExtractEntryError(ZipEntryBase entry, Exception exception) =>
        new(exception, "ExtractEntry", ErrorCategory.NotSpecified, entry);

    internal static ErrorRecord StreamOpenError(ZipEntryFile entry, Exception exception) =>
        new(exception, "StreamOpen", ErrorCategory.NotSpecified, entry);

    internal static ErrorRecord StreamOpenError(string path, Exception exception) =>
        new(exception, "StreamOpen", ErrorCategory.NotSpecified, path);

    internal static ErrorRecord WriteError(object entry, Exception exception) =>
        new(exception, "WriteError", ErrorCategory.WriteError, entry);

    internal static ErrorRecord DuplicatedEntryError(DuplicatedEntryException exception) =>
        new(exception, "DuplicatedEntry", ErrorCategory.WriteError, exception._path);

    internal static ErrorRecord InvalidNameError(string name, ArgumentException exception) =>
        new(exception, "InvalidName", ErrorCategory.InvalidArgument, name);

    internal static ErrorRecord EntryNotFoundError(EntryNotFoundException exception) =>
        new(exception, "EntryNotFound", ErrorCategory.ObjectNotFound, exception._path);

    internal static ErrorRecord EnumerationError(object item, Exception exception) =>
        new(exception, "EnumerationError", ErrorCategory.ReadError, item);

    internal static void ThrowIfNotFound(
    this ZipArchive zip,
    string path,
    string source,
    out ZipArchiveEntry entry)
    {
        if (!zip.TryGetEntry(path, out entry))
        {
            throw EntryNotFoundException.Create(path, source);
        }
    }

    internal static void ThrowIfDuplicate(
        this ZipArchive zip,
        string path,
        string source,
        out string normalizedPath)
    {
        normalizedPath = path.NormalizeFileEntryPath();
        if (zip.TryGetEntry(normalizedPath, out ZipArchiveEntry _))
        {
            throw DuplicatedEntryException.Create(normalizedPath, source);
        }
    }

    internal static void ThrowIfInvalidFileNameChar(this string name, string newname)
    {
        if (name.IndexOfAny(s_InvalidFileNameChar) != -1)
        {
            throw new ArgumentException(
                "Cannot rename the specified target, because it represents a path, " +
                "device name or contains invalid File Name characters.",
                newname);
        }
    }

    internal static void ThrowIfInvalidPathChar(this string path)
    {
        if (path.IndexOfAny(s_InvalidPathChar) != -1)
        {
            throw new ArgumentException(
                $"Path: '{path}' contains invalid path characters.");
        }
    }
}
