using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using ICSharpCode.SharpZipLib.Zip;
using PSCompression.Abstractions.Entries;
using PSCompression.Enum;
using PSCompression.Exceptions;

namespace PSCompression.Extensions;

internal static class ExceptionExtensions
{
    private static readonly char[] s_InvalidFileNameChar = Path.GetInvalidFileNameChars();

    private static readonly char[] s_InvalidPathChar = Path.GetInvalidPathChars();

    extension(string path)
    {
        internal bool WriteErrorIfNotArchive(
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

        internal void ThrowIfInvalidPathChar()
        {
            if (path.IndexOfAny(s_InvalidPathChar) != -1)
            {
                throw new ArgumentException(
                    $"Path: '{path}' contains invalid path characters.");
            }
        }
    }

    internal static ErrorRecord NotDirectoryPath(string path, string paramname) =>
        new(
            new ArgumentException(
                $"Destination path is an existing File: '{path}'.", paramname),
            "NotDirectoryPath", ErrorCategory.InvalidArgument, path);

    extension(ProviderInfo provider)
    {
        internal ErrorRecord ToInvalidProviderError(string path) =>
            new(
                new NotSupportedException(
                    $"The resolved path '{path}' is not a FileSystem path but '{provider.Name}'."),
                "NotFileSystemPath", ErrorCategory.InvalidArgument, path);
    }

    extension(Exception exception)
    {
        internal ErrorRecord ToOpenError(string path) =>
            new(exception, "EntryOpen", ErrorCategory.OpenError, path);

        internal ErrorRecord ToResolvePathError(string path) =>
            new(exception, "ResolvePath", ErrorCategory.NotSpecified, path);

        internal ErrorRecord ToExtractEntryError(object entry) =>
            new(exception, "ExtractEntry", ErrorCategory.NotSpecified, entry);

        internal ErrorRecord ToStreamOpenError(object item) =>
            new(exception, "StreamOpen", ErrorCategory.NotSpecified, item);

        internal ErrorRecord ToWriteError(object? item) =>
            new(exception, "WriteError", ErrorCategory.WriteError, item);

        internal ErrorRecord ToEnumerationError(object item) =>
            new(exception, "EnumerationError", ErrorCategory.ReadError, item);

        internal ErrorRecord ToInvalidArchive(ArchiveType type)
        {
            string basemsg = $"Specified path or stream is not a valid {type} archive, " +
                "might be compressed using an unsupported method, " +
                "or could be corrupted. " +
                "Try specifying the compression format explicitly using the -Algorithm parameter.";

            return new ErrorRecord(
                new InvalidDataException(basemsg, exception),
                "InvalidArchive",
                ErrorCategory.InvalidData,
                null);
        }
    }

    extension(DuplicatedEntryException exception)
    {
        internal ErrorRecord ToDuplicatedEntryError() =>
            new(exception, "DuplicatedEntry", ErrorCategory.WriteError, exception._path);
    }

    extension(InvalidNameException exception)
    {
        internal ErrorRecord ToInvalidNameError(string name) =>
            new(exception, "InvalidName", ErrorCategory.InvalidArgument, name);
    }

    extension(EntryNotFoundException exception)
    {
        internal ErrorRecord ToEntryNotFoundError() =>
            new(exception, "EntryNotFound", ErrorCategory.ObjectNotFound, exception._path);
    }

    extension(ZipArchive zip)
    {
        internal void ThrowIfNotFound(
            string path,
            string source,
            [NotNull] out ZipArchiveEntry? entry)
        {
            if (!zip.TryGetEntry(path, out entry))
            {
                throw EntryNotFoundException.Create(path, source);
            }
        }

        internal void ThrowIfDuplicate(
            string path,
            string source)
        {
            if (zip.TryGetEntry(path, out ZipArchiveEntry? _))
            {
                throw DuplicatedEntryException.Create(path, source);
            }
        }
    }

    extension(ICSharpCode.SharpZipLib.Zip.ZipFile zip)
    {
        internal void ThrowIfNotFound(
            string path,
            string source,
            [NotNull] out ZipEntry? entry)
        {
            if (!zip.TryGetEntry(path, out entry))
            {
                throw EntryNotFoundException.Create(path, source);
            }
        }
    }

    extension(string newname)
    {
        internal void ThrowIfInvalidNameChar()
        {
            if (newname.IndexOfAny(s_InvalidFileNameChar) != -1)
            {
                throw InvalidNameException.Create(newname);
            }
        }
    }

    extension(ZipEntryBase entry)
    {
        internal void ThrowIfFromStream()
        {
            if (entry.FromStream)
            {
                throw new NotSupportedException(
                    "The operation is not supported for entries created from input Stream.");
            }
        }
    }
}
