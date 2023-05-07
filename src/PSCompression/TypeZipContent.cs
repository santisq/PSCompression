namespace PSCompression;

public sealed class ZipContent
{
    public string Source { get; set; }

    public string EntryRelativePath { get; set; }

    public string Content { get; set; }

    internal ZipContent(ZipEntryFile entry)
    {
        Source = entry.Source;
        EntryRelativePath = entry.EntryRelativePath;
        Content = entry.ReadToEnd();
    }
}
