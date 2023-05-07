using System.Collections.Generic;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

public sealed class ZipEntryContent
{
    private readonly ZipEntryFile _sourceEntry;

    private static readonly List<string> _content;

    public string Source => _sourceEntry.Source;

    public string EntryRelativePath => _sourceEntry.EntryRelativePath;

    public object? Content { get; set; }

    internal ZipEntryContent(ZipEntryFile entry) =>
        _sourceEntry = entry;

    static ZipEntryContent() => _content = new();

    internal ZipEntryContent ReadAllText(Encoding? encoding, bool detectEncoding = true)
    {
        if (encoding is null && detectEncoding)
        {
            Content = _sourceEntry.ReadToEnd();
            return this;
        }

        Content = _sourceEntry.ReadToEnd(encoding!, detectEncoding);
        return this;
    }

    internal void ReadLines(PSCmdlet cmdlet, Encoding? encoding, bool detectEncoding = true)
    {
        if(encoding is null && detectEncoding)
        {
            cmdlet.WriteObject(_sourceEntry.ReadLines(), enumerateCollection: true);
            return;
        }

        cmdlet.WriteObject(
            _sourceEntry.ReadLines(encoding!, detectEncoding), enumerateCollection: true);
    }

    internal ZipEntryContent ReadAllLines(Encoding? encoding, bool detectEncoding = true)
    {
        _content.Clear();

        if (encoding is null && detectEncoding)
        {
            _content.AddRange(_sourceEntry.ReadLines());
            Content = _content.ToArray();
            return this;
        }

        _content.AddRange(_sourceEntry.ReadLines(encoding!, detectEncoding));
        Content = _content.ToArray();
        return this;
    }
}
