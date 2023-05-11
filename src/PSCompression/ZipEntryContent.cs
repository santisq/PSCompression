namespace PSCompression;

public sealed class ZipEntryContent
{
    private readonly ZipEntryFile _sourceEntry;

    public string Source => _sourceEntry.Source;

    public string EntryRelativePath => _sourceEntry.EntryRelativePath;

    public object? Content { get; }

    internal ZipEntryContent(ZipEntryFile entry, object content)
    {
        _sourceEntry = entry;
        Content = content;
    }

    public ZipEntryFile GetZipEntry() => _sourceEntry;
}
