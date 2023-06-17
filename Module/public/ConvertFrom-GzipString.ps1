using namespace System.IO
using namespace System.IO.Compression
using namespace System.Text

# .ExternalHelp PSCompression-help.xml
function ConvertFrom-GzipString {
    [CmdletBinding()]
    [Alias('gzipfromstring')]
    [OutputType([string])]
    param(
        [Parameter(Mandatory, ValueFromPipeline)]
        [string[]] $InputObject,

        [Parameter()]
        [PSCompression.EncodingTransformation()]
        [ArgumentCompleter([PSCompression.EncodingCompleter])]
        [Encoding] $Encoding = [UTF8Encoding]::new(),

        [Parameter()]
        [switch] $Raw
    )

    process {
        foreach($string in $InputObject) {
            try {
                $inStream = [MemoryStream]::new([Convert]::FromBase64String($string))
                $gzip = [GZipStream]::new($inStream, [CompressionMode]::Decompress)
                $reader = [StreamReader]::new($gzip, $Encoding, $true)

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
                if($reader -is [System.IDisposable]) {
                    $reader.Dispose()
                }

                if($gzip -is [System.IDisposable]) {
                    $gzip.Dispose()
                }

                if($inStream -is [System.IDisposable]) {
                    $inStream.Dispose()
                }
            }
        }
    }
}
