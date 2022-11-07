﻿using namespace System.IO
using namespace System.IO.Compression
using namespace System.Text

function Expand-GzipArchive {
    <#
    .SYNOPSIS
    Expands a Gzip compressed file from a specified File Path.

    .Parameter Path
    Specifies the path or paths to the Gzip files that you want to expand.
    To specify multiple paths, and include files in multiple locations, use commas to separate the paths.
    This Parameter accepts wildcard characters. Wildcard characters allow you to add all files in a directory to your archive file.

    .PARAMETER LiteralPath
    Specifies the path or paths to the Gzip files that you want to expand.
    Unlike the Path Parameter, the value of LiteralPath is used exactly as it's typed.
    No characters are interpreted as wildcards

    .PARAMETER DestinationPath
    The destination path to where to expand the Gzip file.
    The target folder is created if it does not exist.
    This parameter is Optional, if not used, this function outputs to the Success Stream.

    .PARAMETER Encoding
    Character encoding used when expanding the Gzip content.
    This parameter is only available when expanding to the Success Stream.

    .PARAMETER PassThru
    Outputs the object representing the expanded file.
    This parameter is only available when expanding to a File.

    .LINK
    https://github.com/santisq/PSCompression
    #>

    [CmdletBinding(DefaultParameterSetName='Path')]
    [Alias('gzipfromfile')]
    param(
        [Parameter(ParameterSetName = 'PathToFile', Mandatory, Position = 0, ValueFromPipeline)]
        [Parameter(ParameterSetName = 'Path', Mandatory, Position = 0, ValueFromPipeline)]
        [string[]] $Path,

        [Parameter(ParameterSetName = 'LiteralPathToFile', Mandatory, ValueFromPipelineByPropertyName)]
        [Parameter(ParameterSetName = 'LiteralPath', Mandatory, ValueFromPipelineByPropertyName)]
        [Alias('PSPath')]
        [string[]] $LiteralPath,

        [Parameter(ParameterSetName = 'PathToFile', Position = 1)]
        [Parameter(ParameterSetName = 'LiteralPathToFile', Position = 1)]
        [string] $DestinationPath,

        [Parameter(ParameterSetName = 'Path')]
        [Parameter(ParameterSetName = 'LiteralPath')]
        [EncodingTransformation()]
        [ArgumentCompleter([EncodingCompleter])]
        [Encoding] $Encoding = 'utf8',

        [Parameter(ParameterSetName = 'PathToFile')]
        [Parameter(ParameterSetName = 'LiteralPathToFile')]
        [switch] $PassThru
    )

    begin {
        $ExpectingInput = $null
    }
    process {
        try {
            $isLiteral = $PSBoundParameters.ContainsKey('LiteralPath')
            $paths     = $Path
            if($isLiteral) {
                $paths = $LiteralPath
            }
            $items = $ExecutionContext.InvokeProvider.Item.Get($paths, $true, $isLiteral)

            if(-not $ExpectingInput -and $PSBoundParameters.ContainsKey('DestinationPath')) {
                $ExpectingInput  = $true
                $DestinationPath = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($DestinationPath)
                $null    = [Directory]::CreateDirectory([Path]::GetDirectoryName($DestinationPath))
                $outFile = [File]::Open($DestinationPath, [FileMode]::Append)
            }

            foreach($item in $items) {
                try {
                    $inStream  = [File]::Open($item, [FileMode]::Open)
                    $gzip      = [GZipStream]::new($inStream, [CompressionMode]::Decompress)
                    if($PSBoundParameters.ContainsKey('DestinationPath')) {
                        $gzip.CopyTo($outFile)
                        continue
                    }
                    $reader = [StreamReader]::new($gzip, $Encoding, $true)
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
        catch {
            $PSCmdlet.WriteError($_)
        }
        finally {
            if($outFile -is [IDisposable]) {
                $outFile.Dispose()
            }

            if($PassThru.IsPresent) {
                $outFile.Name -as [FileInfo]
            }
        }
    }
}