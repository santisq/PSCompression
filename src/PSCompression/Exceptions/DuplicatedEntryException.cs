using System.IO;

namespace PSCompression.Exceptions;

public sealed class DuplicatedEntryException : IOException
{
    internal string _path;

    private DuplicatedEntryException(string message, string path)
        : base(message: message)
    {
        _path = path;
    }

    internal static DuplicatedEntryException Create(string path, string source) =>
        new($"An entry with path '{path}' already exists in '{source}'.", path);
}
