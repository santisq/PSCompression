using namespace System.IO
using namespace System.IO.Compression
using namespace System.Text

function ConvertTo-GzipString {
    <#
    .SYNOPSIS
    Creates a Base64 encoded Gzip compressed string from a specified input string or strings.

    .Parameter InputObject
    Specifies the input string or strings to compress.

    .PARAMETER Encoding
    Character encoding used when compressing the Gzip input.

    .PARAMETER DestinationPath
    The destination path to the Gzip file.
    If the file name in DestinationPath doesn't have a `.gzip` file name extension, the function appends the `.gzip` file name extension.

    .PARAMETER CompressionLevel
    Define the compression level that should be used.
    See https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.compressionlevel for details.

    .PARAMETER Raw
    Outputs the compressed bytes to the Success Stream. There is no Base64 Encoding.
    This parameter is meant to be used in combination with `Compress-GzipArchive`.

    .PARAMETER NoNewLine
    The encoded string representations of the input objects are concatenated to form the output.
    No spaces or newlines are inserted between the output strings.
    No newline is added after the last output string.

    .LINK
    https://github.com/santisq/PSCompression
    #>

    [CmdletBinding()]
    [Alias('gziptostring')]
    param(
        [AllowEmptyString()]
        [Parameter(Mandatory, ValueFromPipeline)]
        [string[]] $InputObject,

        [Parameter()]
        [EncodingTransformation()]
        [ArgumentCompleter([EncodingCompleter])]
        [Encoding] $Encoding = 'utf-8',

        [Parameter()]
        [CompressionLevel] $CompressionLevel = 'Optimal',

        [Parameter()]
        [switch] $Raw,

        [Parameter()]
        [switch] $NoNewLine
    )

    begin {
        $inStream = [MemoryStream]::new()
        $newLine  = $Encoding.GetBytes([Environment]::NewLine)
    }
    process {
        foreach($string in $InputObject) {
            $bytes = $Encoding.GetBytes($string)
            $inStream.Write($bytes, 0, $bytes.Length)
            if($NoNewLine.IsPresent) {
                continue
            }
            $inStream.Write($newLine, 0, $newLine.Length)
        }
    }
    end {
        try {
            $outStream = [MemoryStream]::new()
            $gzip      = [GZipStream]::new($outStream, [CompressionMode]::Compress, $CompressionLevel)
            $inStream.Flush()
            $inStream.WriteTo($gzip)
        }
        catch {
            $PSCmdlet.WriteError($_)
        }
        finally {
            $gzip, $outStream, $inStream | ForEach-Object Dispose
        }

        try {
            if($Raw.IsPresent) {
                return $PSCmdlet.WriteObject($outStream.ToArray())
            }
            [Convert]::ToBase64String($outStream.ToArray())
        }
        catch {
            $PSCmdlet.WriteError($_)
        }
    }
}