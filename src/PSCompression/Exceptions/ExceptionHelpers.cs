using System;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Extensions;

namespace PSCompression.Exceptions;

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

    internal static ErrorRecord ToOpenError(this Exception exception, string path) =>
        new(exception, "ZipOpen", ErrorCategory.OpenError, path);

    internal static ErrorRecord ToResolvePathError(this Exception exception, string path) =>
        new(exception, "ResolvePath", ErrorCategory.NotSpecified, path);

    internal static ErrorRecord ToExtractEntryError(this Exception exception, ZipEntryBase entry) =>
        new(exception, "ExtractEntry", ErrorCategory.NotSpecified, entry);

    internal static ErrorRecord ToStreamOpenError(this Exception exception, ZipEntryFile entry) =>
        new(exception, "StreamOpen", ErrorCategory.NotSpecified, entry);

    internal static ErrorRecord ToStreamOpenError(this Exception exception, string path) =>
        new(exception, "StreamOpen", ErrorCategory.NotSpecified, path);

    internal static ErrorRecord ToWriteError(this Exception exception, object? item) =>
        new(exception, "WriteError", ErrorCategory.WriteError, item);

    internal static ErrorRecord ToDuplicatedEntryError(this DuplicatedEntryException exception) =>
        new(exception, "DuplicatedEntry", ErrorCategory.WriteError, exception._path);

    internal static ErrorRecord ToInvalidNameError(this InvalidNameException exception, string name) =>
        new(exception, "InvalidName", ErrorCategory.InvalidArgument, name);

    internal static ErrorRecord ToEntryNotFoundError(this EntryNotFoundException exception) =>
        new(exception, "EntryNotFound", ErrorCategory.ObjectNotFound, exception._path);

    internal static ErrorRecord ToEnumerationError(this Exception exception, object item) =>
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
        string source)
    {
        if (zip.TryGetEntry(path, out ZipArchiveEntry _))
        {
            throw DuplicatedEntryException.Create(path, source);
        }
    }

    internal static void ThrowIfInvalidNameChar(this string newname)
    {
        if (newname.IndexOfAny(s_InvalidFileNameChar) != -1)
        {
            throw InvalidNameException.Create(newname);
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
