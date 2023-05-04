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
            $sb     = [StringBuilder]::new()
            $stream = $File.OpenRead()
            $marker = 0
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
                            $null   = $subStream.Seek($stream.Position - 3, [SeekOrigin]::Begin)
                            $gzip   = [GZipStream]::new($subStream, [CompressionMode]::Decompress)
                            $reader = [StreamReader]::new($gzip)

                            if($PSBoundParameters.ContainsKey('OutStream')) {
                                $gzip.CopyTo($OutStream)
                                continue
                            }

                            if($Raw.IsPresent) {
                                while(-not $reader.EndOfStream) {
                                    $sb = $sb.AppendLine($reader.ReadLine())
                                    continue
                                }
                            }

                            while(-not $reader.EndOfStream) {
                                $reader.ReadLine()
                            }
                        }
                        finally {
                            $reader, $gzip, $subStream | ForEach-Object Dispose
                        }
                    }
                }
            }
            if($Raw.IsPresent) {
                $sb.ToString()
            }
        }
        finally {
            if($stream -is [IDisposable]) {
                $stream.Dispose()
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
            $inStream  = $File.OpenRead()
            $gzip      = [GZipStream]::new($inStream, [CompressionMode]::Decompress)

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
            $gzip, $reader, $inStream | ForEach-Object Dispose
        }
    }
}
