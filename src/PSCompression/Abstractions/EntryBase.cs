using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace PSCompression.Abstractions;

public abstract class EntryBase(string source)
{
    protected Stream? _stream;

    protected string? _formatDirectoryPath;

    internal string? FormatDirectoryPath { get => _formatDirectoryPath ??= GetFormatDirectoryPath(); }

    [MemberNotNullWhen(true, nameof(_stream))]
    internal bool FromStream { get => _stream is not null; }

    public string Source { get; } = source;

    public abstract string Name { get; protected set; }

    public abstract string RelativePath { get; }

    public abstract DateTime LastWriteTime { get; }

    public abstract long Length { get; internal set; }

    public abstract EntryType Type { get; }

    protected abstract string GetFormatDirectoryPath();

    public override string ToString() => RelativePath;
}
