using System.IO;

namespace PSCompression;

public sealed class EntryNotFoundException : IOException
{
    internal string _path;

    private EntryNotFoundException(string message, string path)
        : base(message: message)
    {
        _path = path;
    }

    internal static EntryNotFoundException Create(string path, string source) =>
        new EntryNotFoundException(
            $"Cannot find '{path}' in '{source}'.",
            path: path);
}
