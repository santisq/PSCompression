using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using PSCompression.Abstractions;
using PSCompression.Extensions;

namespace PSCompression.Exceptions;

internal static class ExceptionHelper
{
    private static readonly char[] s_InvalidFileNameChar = Path.GetInvalidFileNameChars();

    private static readonly char[] s_InvalidPathChar = Path.GetInvalidPathChars();

    internal static bool WriteErrorIfNotArchive(
        this string path,
        string paramname,
        PSCmdlet cmdlet,
        bool isTerminating = false)
    {
        if (File.Exists(path))
        {
            return false;
        }

        ArgumentException exception = new(
            $"The specified path '{path}' does not exist or is a Directory.",
            paramname);

        ErrorRecord error = new(exception, "NotArchivePath", ErrorCategory.InvalidArgument, path);

        if (isTerminating)
        {
            cmdlet.ThrowTerminatingError(error);
        }

        cmdlet.WriteError(error);
        return true;
    }


    internal static ErrorRecord NotDirectoryPath(string path, string paramname) =>
        new(
            new ArgumentException(
                $"Destination path is an existing File: '{path}'.", paramname),
            "NotDirectoryPath", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord ToInvalidProviderError(this ProviderInfo provider, string path) =>
        new(
            new NotSupportedException(
                $"The resolved path '{path}' is not a FileSystem path but '{provider.Name}'."),
            "NotFileSystemPath", ErrorCategory.InvalidArgument, path);

    internal static ErrorRecord ToOpenError(this Exception exception, string path) =>
        new(exception, "EntryOpen", ErrorCategory.OpenError, path);

    internal static ErrorRecord ToResolvePathError(this Exception exception, string path) =>
        new(exception, "ResolvePath", ErrorCategory.NotSpecified, path);

    internal static ErrorRecord ToExtractEntryError(this Exception exception, EntryBase entry) =>
        new(exception, "ExtractEntry", ErrorCategory.NotSpecified, entry);

    internal static ErrorRecord ToStreamOpenError(this Exception exception, ZipEntryBase entry) =>
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

    internal static ErrorRecord ToInvalidArchive(
        this Exception exception,
        ArchiveType type,
        bool isStream = false)
    {
        string basemsg = $"Specified path or stream is not a valid {type} archive, " +
            "might be compressed using an unsupported method, " +
            "or could be corrupted.";

        if (type is ArchiveType.tar && isStream)
        {
            basemsg += " When reading a tar archive from a stream, " +
                "use the -Algorithm parameter to specify the compression type.";
        }

        return new ErrorRecord(
            new InvalidDataException(basemsg, exception),
            "InvalidArchive", ErrorCategory.InvalidData, null);
    }


    internal static void ThrowIfNotFound(
        this ZipArchive zip,
        string path,
        string source,
        [NotNull] out ZipArchiveEntry? entry)
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
        if (zip.TryGetEntry(path, out ZipArchiveEntry? _))
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

    internal static void ThrowIfFromStream(this ZipEntryBase entry)
    {
        if (entry.FromStream)
        {
            throw new NotSupportedException(
                "The operation is not supported for entries created from input Stream.");
        }
    }
}
