using System.IO;

namespace PSCompression.Exceptions;

public sealed class EntryNotFoundException : IOException
{
    internal string _path;

    private EntryNotFoundException(string message, string path)
        : base(message: message)
    {
        _path = path;
    }

    internal static EntryNotFoundException Create(string path, string source) =>
        new($"Cannot find an entry with path: '{path}' in '{source}'.", path);
}
