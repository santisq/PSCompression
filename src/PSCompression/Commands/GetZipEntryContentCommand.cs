using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

[Cmdlet(VerbsCommon.Get, "ZipEntryContent", DefaultParameterSetName = "Raw")]
[OutputType(typeof(ZipEntryContent))]
[Alias("gczip")]
public sealed class GetZipEntryContentCommand : PSCmdlet
{
    private readonly List<string> _content = new();

    [Parameter(Mandatory = true, ValueFromPipeline = true)]
    public ZipEntryFile[] InputObject { get; set; } = null!;

    [Parameter(ParameterSetName = "StringValue")]
    [ArgumentCompleter(typeof(EncodingCompleter))]
    [EncodingTransformation]
    public Encoding Encoding { get; set; } = Encoding.UTF8;

    [Parameter(ParameterSetName = "Raw")]
    public SwitchParameter Raw { get; set; }

    [Parameter(ParameterSetName = "Stream")]
    public SwitchParameter Stream { get; set; }

    private void StreamRead(StreamReader reader)
    {
        while (!reader.EndOfStream)
        {
            WriteObject(reader.ReadLine());
        }
    }

    protected override void ProcessRecord()
    {
        foreach (ZipEntryFile entry in InputObject)
        {
            try
            {
                using ZipEntryStream stream = entry.OpenRead();
                using StreamReader reader = new(stream, Encoding);

                if (Stream.IsPresent)
                {
                    StreamRead(reader);
                    return;
                }

                if (Raw.IsPresent)
                {
                    WriteObject(new ZipEntryContent(entry, reader.ReadToEnd()));
                    return;
                }

                while (!reader.EndOfStream)
                {
                    _content.Add(reader.ReadLine());
                }

                WriteObject(new ZipEntryContent(entry, _content.ToArray()));
                _content.Clear();
            }
            catch (PipelineStoppedException)
            {
                throw;
            }
            catch (Exception e)
            {
                WriteError(new ErrorRecord(
                    e, "EntryOpen", ErrorCategory.OpenError, entry));
            }
        }
    }
}
