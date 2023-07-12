using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Text;

namespace PSCompression;

public static class GzipReaderOps
{
    private const byte GzipPreamble1 = 0x1f;

    private const byte GzipPreamble2 = 0x8b;

    private const byte GzipPreamble3 = 0x08;

    public static void GetContent(
        string path,
        bool isCoreCLR,
        bool raw,
        Encoding encoding,
        PSCmdlet cmdlet)
    {
        if (isCoreCLR)
        {
            using FileStream fs = File.OpenRead(path);
            using GZipStream gzip = new(fs, CompressionMode.Decompress);

            if (raw)
            {
                ReadToEnd(gzip, encoding, cmdlet);
                return;
            }

            ReadLines(gzip, encoding, cmdlet);
            return;
        }

        using MemoryStream stream = GetFrameworkStream(path);

        if (raw)
        {
            ReadToEnd(stream, encoding, cmdlet);
            return;
        }

        ReadLines(stream, encoding, cmdlet);
    }

    private static void ReadLines(
        Stream stream,
        Encoding encoding,
        PSCmdlet cmdlet)
    {
        using StreamReader reader = new(stream, encoding);

        while (!reader.EndOfStream)
        {
            cmdlet.WriteObject(reader.ReadLine());
        }
    }

    private static void ReadToEnd(
        Stream stream,
        Encoding encoding,
        PSCmdlet cmdlet)
    {
        using StreamReader reader = new(stream, encoding);
        cmdlet.WriteObject(reader.ReadToEnd());
    }

    // this stuff is to make this work reading appended gzip streams in .net framework
    // i hate it :(
    private static MemoryStream GetFrameworkStream(string path)
    {
        int marker = 0;
        int b;
        using FileStream fs = File.OpenRead(path);
        MemoryStream outmem = new();

        while ((b = fs.ReadByte()) != -1)
        {
            if (marker == 0 && (byte)b == GzipPreamble1)
            {
                marker++;
                continue;
            }

            if (marker == 1)
            {
                if ((byte)b == GzipPreamble2)
                {
                    marker++;
                    continue;
                }

                marker = 0;
            }

            if (marker == 2)
            {
                marker = 0;

                if ((byte)b == GzipPreamble3)
                {
                    AppendBytes(path, outmem, fs.Position - 3);
                }
            }
        }

        outmem.Seek(0, SeekOrigin.Begin);
        return outmem;
    }

    private static void AppendBytes(string path, MemoryStream outmem, long pos)
    {
        using FileStream substream = File.OpenRead(path);
        substream.Seek(pos, SeekOrigin.Begin);
        using GZipStream gzip = new(substream, CompressionMode.Decompress);
        gzip.CopyTo(outmem);
    }
}
