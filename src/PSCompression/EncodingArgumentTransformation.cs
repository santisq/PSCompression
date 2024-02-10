using System;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Text;

namespace PSCompression;

public sealed class EncodingTransformation : ArgumentTransformationAttribute
{
    public override object Transform(
        EngineIntrinsics engineIntrinsics,
        object inputData)
    {
        return inputData switch
        {
            Encoding enc => enc,
            int num => Encoding.GetEncoding(num),
            string str => ParseStringEncoding(str),
            _ => throw new ArgumentTransformationMetadataException(
                $"Could not convert input '{inputData}' to a valid Encoding object."),
        };
    }

    private Encoding ParseStringEncoding(string str)
    {
        return str.ToLowerInvariant() switch
        {
            "ascii" => new ASCIIEncoding(),
            "bigendianunicode" => new UnicodeEncoding(true, true),
            "bigendianutf32" => new UTF32Encoding(true, true),
            "oem" => Console.OutputEncoding,
            "unicode" => new UnicodeEncoding(),
            "utf8" => new UTF8Encoding(false),
            "utf8bom" => new UTF8Encoding(true),
            "utf8nobom" => new UTF8Encoding(false),
            "utf32" => new UTF32Encoding(),
            "ansi" => Encoding.GetEncoding(GetACP()),
            _ => Encoding.GetEncoding(str),
        };
    }

    [DllImport("Kernel32.dll")]
    private static extern int GetACP();
}
