using namespace System.IO
using namespace System.IO.Compression
using namespace System.Text

# .ExternalHelp PSCompression-help.xml
function Expand-GzipArchive {
    [CmdletBinding(PositionalBinding = $false)]
    [Alias('gzipfromfile')]
    [OutputType([string], ParameterSetName = ('Path', 'LiteralPath'))]
    [OutputType([System.IO.FileInfo], ParameterSetName = ('PathDestination', 'LiteralPathDestination'))]
    param(
        [Parameter(ParameterSetName = 'Path', Mandatory, Position = 0, ValueFromPipeline)]
        [Parameter(ParameterSetName = 'PathDestination', Mandatory, Position = 0, ValueFromPipeline)]
        [string[]] $Path,

        [Parameter(ParameterSetName = 'LiteralPath', Mandatory, ValueFromPipelineByPropertyName)]
        [Parameter(ParameterSetName = 'LiteralPathDestination', Mandatory, ValueFromPipelineByPropertyName)]
        [Alias('PSPath')]
        [string[]] $LiteralPath,

        [Parameter(Mandatory, ParameterSetName = 'PathDestination')]
        [Parameter(Mandatory, ParameterSetName = 'LiteralPathDestination')]
        [string] $DestinationPath,

        [Parameter(ParameterSetName = 'Path')]
        [Parameter(ParameterSetName = 'LiteralPath')]
        [PSCompression.EncodingTransformation()]
        [ArgumentCompleter([PSCompression.EncodingCompleter])]
        [Encoding] $Encoding = [UTF8Encoding]::new(),

        [Parameter(ParameterSetName = 'Path')]
        [Parameter(ParameterSetName = 'LiteralPath')]
        [switch] $Raw,

        [Parameter(ParameterSetName = 'PathDestination')]
        [Parameter(ParameterSetName = 'LiteralPathDestination')]
        [switch] $PassThru
    )

    begin {
        $ExpectingInput = $null
        $params = @{
            Raw      = $Raw.IsPresent
            Encoding = $Encoding
        }
    }
    process {
        try {
            $isLiteral = $PSBoundParameters.ContainsKey('LiteralPath')
            $paths = $Path

            if($isLiteral) {
                $paths = $LiteralPath
            }

            $items = $PSCmdlet.InvokeProvider.Item.Get($paths, $true, $isLiteral)

            if(-not $ExpectingInput -and $PSBoundParameters.ContainsKey('DestinationPath')) {
                $ExpectingInput = $true
                $DestinationPath = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($DestinationPath)
                $null = [Directory]::CreateDirectory([Path]::GetDirectoryName($DestinationPath))
                $params['OutStream'] = [File]::Open($DestinationPath, [FileMode]::Append)
            }

            # Had to do this to read appended Gzip content in .NET Framework...
            if($IsCoreCLR) {
                return $items | GzipCoreReader @params
            }

            $items | GzipFrameworkReader @params
        }
        catch {
            $PSCmdlet.WriteError($_)
        }
        finally {
            if($params['OutStream'] -is [IDisposable]) {
                $params['OutStream'].Dispose()
            }

            if($PassThru.IsPresent) {
                $params['OutStream'].Name -as [FileInfo]
            }
        }
    }
}
