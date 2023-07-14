using System.IO;
using System.IO.Compression;
using System.Management.Automation;
using System.Runtime.Serialization;
using System.Text;

namespace PSCompression;

internal static class GzipReaderOps
{
    private static readonly byte[] gzipPreamble = new byte[3]
    {
        0x1f,
        0x8b,
        0x08
    };

    internal static void CopyTo(
        string path,
        bool isCoreCLR,
        FileStream destination)
    {
        if (isCoreCLR)
        {
            using FileStream fs = File.OpenRead(path);
            using GZipStream gzip = new(fs, CompressionMode.Decompress);
            gzip.CopyTo(destination);
            return;
        }

        using MemoryStream mem = GetFrameworkStream(path);
        mem.CopyTo(destination);
    }

    internal static void GetContent(
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

        byte[] preamble = new byte[3];
        fs.Read(preamble, 0, 3);

        for (int i = 0; i < 3; i++)
        {
            if(preamble[i] != gzipPreamble[i])
            {
                throw new InvalidDataContractException(
                    "The archive entry was compressed using an unsupported compression method.");
            }
        }

        fs.Seek(0, SeekOrigin.Begin);

        MemoryStream outmem = new();

        while ((b = fs.ReadByte()) != -1)
        {
            if (marker == 0 && (byte)b == gzipPreamble[marker])
            {
                marker++;
                continue;
            }

            if (marker == 1)
            {
                if ((byte)b == gzipPreamble[marker])
                {
                    marker++;
                    continue;
                }

                marker = 0;
            }

            if (marker == 2)
            {
                if ((byte)b == gzipPreamble[marker])
                {
                    CopyTo(path, outmem, fs.Position - 3);
                }

                marker = 0;
            }
        }

        outmem.Seek(0, SeekOrigin.Begin);
        return outmem;
    }

    private static void CopyTo(string path, MemoryStream outmem, long pos)
    {
        using FileStream substream = File.OpenRead(path);
        substream.Seek(pos, SeekOrigin.Begin);
        using GZipStream gzip = new(substream, CompressionMode.Decompress);
        gzip.CopyTo(outmem);
    }
}
