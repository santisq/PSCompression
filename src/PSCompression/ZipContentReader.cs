using System.IO;
using System.Management.Automation;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using PSCompression.Abstractions;
using PSCompression.Extensions;

namespace PSCompression;

internal sealed class ZipContentReader : ZipContentOpsBase<ZipFile>
{
    internal ZipContentReader(ZipFile zip) : base(zip)
    { }

    internal void StreamBytes(
        ZipEntryFile entry,
        int bufferSize,
        PSCmdlet cmdlet)
    {
        int bytes;
        using Stream entryStream = entry.Open(ZipArchive);
        Buffer ??= new byte[bufferSize];

        while ((bytes = entryStream.Read(Buffer, 0, bufferSize)) > 0)
        {
            for (int i = 0; i < bytes; i++)
            {
                cmdlet.WriteObject(Buffer[i]);
            }
        }
    }

    internal void ReadAllBytes(ZipEntryFile entry, PSCmdlet cmdlet)
    {
        using Stream entryStream = entry.Open(ZipArchive);
        using MemoryStream mem = new();

        entryStream.CopyTo(mem);
        cmdlet.WriteObject(mem.ToArray());
    }

    internal void StreamLines(
        ZipEntryFile entry,
        Encoding encoding,
        PSCmdlet cmdlet)
    {
        using Stream entryStream = entry.Open(ZipArchive);
        using StreamReader reader = new(entryStream, encoding);
        reader.WriteLinesToPipeline(cmdlet);
    }

    internal void ReadToEnd(
        ZipEntryFile entry,
        Encoding encoding,
        PSCmdlet cmdlet)
    {
        using Stream entryStream = entry.Open(ZipArchive);
        using StreamReader reader = new(entryStream, encoding);
        reader.WriteAllTextToPipeline(cmdlet);
    }
}
