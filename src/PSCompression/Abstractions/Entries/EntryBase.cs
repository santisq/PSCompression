using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using PSCompression.Enum;

namespace PSCompression.Abstractions.Entries;

public abstract class EntryBase(string source)
{
    protected Stream? Stream { get; set; }

    internal abstract string? FormatDirectoryPath { get; }

    [MemberNotNullWhen(true, nameof(Stream))]
    internal bool FromStream { get => Stream is not null; }

    public string Source { get; } = source;

    public abstract string? Name { get; protected set; }

    public abstract string RelativePath { get; }

    public abstract DateTime? LastWriteTime { get; }

    public abstract long Length { get; internal set; }

    public abstract EntryType Type { get; }

    public override string ToString() => RelativePath;
}
