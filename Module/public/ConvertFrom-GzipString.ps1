using namespace System.IO
using namespace System.IO.Compression
using namespace System.Text

function ConvertFrom-GzipString {
    <#
    .SYNOPSIS
    Expands a Base64 encoded Gzip compressed input strings.

    .PARAMETER InputObject
    The Base64 encoded Gzip compressed string or strings to expand.

    .PARAMETER Encoding
    Character encoding used when expanding the Gzip strings.

    .PARAMETER Raw
    Outputs the expanded string as a single string with newlines preserved.
    By default, newline characters in the expanded string are used as delimiters to separate the input into an array of strings.

    .LINK
    https://github.com/santisq/PSCompression
    #>

    [CmdletBinding()]
    [Alias('gzipfromstring')]
    param(
        [Parameter(Mandatory, ValueFromPipeline)]
        [string[]] $InputObject,

        [Parameter()]
        [EncodingTransformation()]
        [ArgumentCompleter([EncodingCompleter])]
        [Encoding] $Encoding = 'utf8',

        [Parameter()]
        [switch] $Raw
    )

    process {
        foreach($string in $InputObject) {
            try {
                $inStream  = [MemoryStream]::new([Convert]::FromBase64String($string))
                $gzip      = [GZipStream]::new($inStream, [CompressionMode]::Decompress)
                $reader    = [StreamReader]::new($gzip, $Encoding, $true)

                if($Raw.IsPresent) {
                    return $reader.ReadToEnd()
                }

                while(-not $reader.EndOfStream) {
                    $reader.ReadLine()
                }
            }
            catch {
                $PSCmdlet.WriteError($_)
            }
            finally {
                $reader, $gzip, $inStream | ForEach-Object Dispose
            }
        }
    }
}
