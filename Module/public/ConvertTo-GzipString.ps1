using namespace System.IO
using namespace System.IO.Compression
using namespace System.Text

# .ExternalHelp PSCompression-help.xml
function ConvertTo-GzipString {
    [CmdletBinding()]
    [Alias('gziptostring')]
    [OutputType([byte], ParameterSetName = 'ByteStream')]
    [OutputType([string])]
    param(
        [AllowEmptyString()]
        [Parameter(Mandatory, ValueFromPipeline)]
        [string[]] $InputObject,

        [Parameter()]
        [EncodingTransformation()]
        [ArgumentCompleter([EncodingCompleter])]
        [Encoding] $Encoding = 'utf8',

        [Parameter()]
        [CompressionLevel] $CompressionLevel = 'Optimal',

        [Parameter(ParameterSetName = 'ByteStream')]
        [switch] $AsByteStream,

        [Parameter()]
        [switch] $NoNewLine
    )

    begin {
        $inStream = [MemoryStream]::new()
        $newLine = $Encoding.GetBytes([Environment]::NewLine)
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
            $gzip = [GZipStream]::new($outStream, [CompressionMode]::Compress, $CompressionLevel)
            $inStream.Flush()
            $inStream.WriteTo($gzip)
        }
        catch {
            $PSCmdlet.WriteError($_)
        }
        finally {
            if($gzip -is [System.IDisposable]) {
                $gzip.Dispose()
            }

            if($outStream -is [System.IDisposable]) {
                $outStream.Dispose()
            }

            if($inStream -is [System.IDisposable]) {
                $inStream.Dispose()
            }
        }

        try {
            if($AsByteStream.IsPresent) {
                return $PSCmdlet.WriteObject($outStream.ToArray())
            }

            [Convert]::ToBase64String($outStream.ToArray())
        }
        catch {
            $PSCmdlet.WriteError($_)
        }
    }
}
