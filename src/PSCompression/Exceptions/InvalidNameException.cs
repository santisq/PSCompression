using System;

namespace PSCompression.Exceptions;

public sealed class InvalidNameException : ArgumentException
{
    internal string _name;

    private InvalidNameException(string message, string name)
        : base(message: message)
    {
        _name = name;
    }

    internal static InvalidNameException Create(string name) =>
        new("Cannot rename the specified target, because it represents a path, "
            + "device name or contains invalid File Name characters.", name);
}
