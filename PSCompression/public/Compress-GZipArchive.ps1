using namespace System.IO
using namespace System.IO.Compression
using namespace System.Text
using namespace System.Management.Automation
function Compress-GzipArchive {
    <#
    .SYNOPSIS
    Creates a Gzip compressed file from specified File Paths or input Bytes.

    .Parameter Path
    Specifies the path or paths to the files that you want to add to the Gzip archive file.
    To specify multiple paths, and include files in multiple locations, use commas to separate the paths.
    This Parameter accepts wildcard characters. Wildcard characters allow you to add all files in a directory to your archive file.

    .PARAMETER LiteralPath
    Specifies the path or paths to the files that you want to add to the Gzip archive file.
    Unlike the Path Parameter, the value of LiteralPath is used exactly as it's typed.
    No characters are interpreted as wildcards

    .PARAMETER InputBytes
    Takes the bytes from pipeline and adds to the Gzip archive file.
    This parameter is meant to be used in combination with `ConvertTo-GzipString -Raw`.

    .PARAMETER DestinationPath
    The destination path to the Gzip file.
    If the file name in DestinationPath doesn't have a `.gzip` file name extension, the function appends the `.gzip` file name extension.

    .PARAMETER CompressionLevel
    Define the compression level that should be used.
    See https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.compressionlevel for details.

    .PARAMETER Update
    Appends to the existing Gzip file.

    .PARAMETER Force
    Replaces an existing Gzip file with a new one. All contents will be lost.

    .PARAMETER PassThru
    Outputs the object representing the compressed file. The function produces no output by default.

    .LINK
    https://github.com/santisq/PSCompression
    #>

    [CmdletBinding(DefaultParameterSetName='Path')]
    [Alias('gziptofile')]
    param(
        [Parameter(ParameterSetName = 'PathWithUpdate', Mandatory, Position = 0, ValueFromPipeline)]
        [Parameter(ParameterSetName = 'PathWithForce', Mandatory, Position = 0, ValueFromPipeline)]
        [Parameter(ParameterSetName = 'Path', Mandatory, Position = 0, ValueFromPipeline)]
        [string[]] $Path,

        [Parameter(ParameterSetName = 'LiteralPathWithUpdate', Mandatory, ValueFromPipelineByPropertyName)]
        [Parameter(ParameterSetName = 'LiteralPathWithForce', Mandatory, ValueFromPipelineByPropertyName)]
        [Parameter(ParameterSetName = 'LiteralPath', Mandatory, ValueFromPipelineByPropertyName)]
        [Alias('PSPath')]
        [string[]] $LiteralPath,

        [Parameter(ParameterSetName = 'RawBytesWithUpdate', Mandatory, ValueFromPipeline)]
        [Parameter(ParameterSetName = 'RawBytesWithForce', Mandatory, ValueFromPipeline)]
        [Parameter(ParameterSetName = 'RawBytes', Mandatory, ValueFromPipeline)]
        [byte[]] $InputBytes,

        [Parameter(Position = 1, Mandatory)]
        [string] $DestinationPath,

        [Parameter()]
        [CompressionLevel] $CompressionLevel = 'Optimal',

        [Parameter(ParameterSetName = 'RawBytesWithUpdate', Mandatory)]
        [Parameter(ParameterSetName = 'PathWithUpdate', Mandatory)]
        [Parameter(ParameterSetName = 'LiteralPathWithUpdate', Mandatory)]
        [switch] $Update,

        [Parameter(ParameterSetName = 'RawBytesWithForce', Mandatory)]
        [Parameter(ParameterSetName = 'PathWithForce', Mandatory)]
        [Parameter(ParameterSetName = 'LiteralPathWithForce', Mandatory)]
        [switch] $Force,

        [Parameter()]
        [switch] $PassThru
    )

    begin {
        if($Force.IsPresent) {
            $fsMode = [FileMode]::Create
        }
        elseif($Update.IsPresent) {
            $fsMode = [FileMode]::Append
        }
        else {
            $fsMode = [FileMode]::CreateNew
        }
        $ExpectingInput = $null
    }
    process {
        try {
            if($withPath = -not $PSBoundParameters.ContainsKey('InputBytes')) {
                $isLiteral = $PSBoundParameters.ContainsKey('LiteralPath')
                $paths     = $Path
                if($isLiteral) {
                    $paths = $LiteralPath
                }
                $items = $ExecutionContext.InvokeProvider.Item.Get($paths, $true, $isLiteral)

                if(-not $items) {
                    foreach($path in $paths) {
                        $PSCmdlet.WriteError([ErrorRecord]::new(
                            [ItemNotFoundException] "Cannot find path '$path' because it does not exist.",
                            'PathNotFound',
                            [ErrorCategory]::ObjectNotFound,
                            $DestinationPath
                        ))
                    }
                }
            }

            if(-not $expectingInput) {
                $expectingInput  = $true
                $DestinationPath = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($DestinationPath)
                if([Path]::GetExtension($DestinationPath) -ne '.gzip') {
                    $DestinationPath = $DestinationPath + '.gzip'
                }
                $null      = [Directory]::CreateDirectory([Path]::GetDirectoryName($DestinationPath))
                $outStream = [File]::Open($DestinationPath, $fsMode)
                $gzip      = [GZipStream]::new($outStream, [CompressionMode]::Compress, $CompressionLevel)

                if(-not $withPath) {
                    $inStream = [MemoryStream]::new($InputBytes)
                }
            }

            foreach($item in $items) {
                try {
                    $inStream = $item.OpenRead()
                    $inStream.CopyTo($gzip)
                }
                catch {
                    $PSCmdlet.WriteError($_)
                }
                finally {
                    if($inStream -is [IDisposable]) {
                        $inStream.Dispose()
                    }
                }
            }
        }
        catch {
            $gzip, $outStream, $inStream | ForEach-Object Dispose
            $PSCmdlet.ThrowTerminatingError($_)
        }
    }
    end {
        try {
            if(-not $withPath) {
                $inStream.Flush()
                $inStream.CopyTo($outStream)
            }
        }
        catch {
            $PSCmdlet.ThrowTerminatingError($_)
        }
        finally {
            $gzip, $outStream, $inStream | ForEach-Object Dispose

            if($PassThru.IsPresent) {
                $outStream.Name -as [FileInfo]
            }
        }
    }
}
