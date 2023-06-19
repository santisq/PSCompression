using namespace System.IO
using namespace System.IO.Compression
using namespace System.Text

function GzipFrameworkReader {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, ValueFromPipeline)]
        [FileInfo] $File,

        [Parameter()]
        [FileStream] $OutStream,

        [Parameter()]
        [switch] $Raw,

        [Parameter(Mandatory)]
        [Encoding] $Encoding
    )

    process {
        try {
            # Credits to jborean - https://github.com/jborean93 for this craziness
            $stream = $File.OpenRead()
            $marker = 0
            $outmem = [MemoryStream]::new()

            while (($b = $stream.ReadByte()) -ne -1) {
                if ($marker -eq 0 -and $b -eq 0x1F) {
                    $marker += 1
                }
                elseif ($marker -eq 1) {
                    if ($b -eq 0x8B) {
                        $marker += 1
                    }
                    else {
                        $marker = 0
                    }
                }
                elseif ($marker -eq 2) {
                    $marker = 0
                    if ($b -eq 0x08) {
                        try {
                            $subStream = $File.OpenRead()
                            $null = $subStream.Seek($stream.Position - 3, [SeekOrigin]::Begin)
                            $gzip = [GZipStream]::new($subStream, [CompressionMode]::Decompress)

                            if($PSBoundParameters.ContainsKey('OutStream')) {
                                $gzip.CopyTo($OutStream)
                                continue
                            }

                            $gzip.CopyTo($outmem)
                        }
                        finally {
                            if($gzip -is [System.IDisposable]) {
                                $gzip.Dispose()
                            }

                            if($subStream -is [System.IDisposable]) {
                                $subStream.Dispose()
                            }
                        }
                    }
                }
            }

            $null = $outmem.Seek(0, [SeekOrigin]::Begin)
            $reader = [StreamReader]::new($outmem, $Encoding)

            if($Raw.IsPresent) {
                return $reader.ReadToEnd()
            }

            while(-not $reader.EndOfStream) {
                $reader.ReadLine()
            }
        }
        finally {
            if($stream -is [IDisposable]) {
                $stream.Dispose()
            }

            if($reader -is [IDisposable]) {
                $reader.Dispose()
            }

            if($outmem -is [IDisposable]) {
                $outmem.Dispose()
            }
        }
    }
}

function GzipCoreReader {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory, ValueFromPipeline)]
        [FileInfo] $File,

        [Parameter()]
        [FileStream] $OutStream,

        [Parameter()]
        [switch] $Raw,

        [Parameter(Mandatory)]
        [Encoding] $Encoding
    )

    process {
        try {
            $inStream = $File.OpenRead()
            $gzip = [GZipStream]::new($inStream, [CompressionMode]::Decompress)

            if($PSBoundParameters.ContainsKey('OutStream')) {
                return $gzip.CopyTo($OutStream)
            }

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
            if($gzip -is [System.IDisposable]) {
                $gzip.Dispose()
            }

            if($reader -is [System.IDisposable]) {
                $reader.Dispose()
            }

            if($inStream -is [System.IDisposable]) {
                $inStream.Dispose()
            }
        }
    }
}
