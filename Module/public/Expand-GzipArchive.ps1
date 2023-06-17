using namespace System.IO
using namespace System.IO.Compression
using namespace System.Text

# .ExternalHelp PSCompression-help.xml
function Expand-GzipArchive {
    [CmdletBinding(DefaultParameterSetName = 'Path')]
    [Alias('gzipfromfile')]
    [OutputType([string], ParameterSetName = ('Path', 'LiteralPath'))]
    [OutputType([System.IO.FileInfo], ParameterSetName = ('PathToFile', 'LiteralPathToFile'))]
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
        [PSCompression.EncodingTransformation()]
        [ArgumentCompleter([PSCompression.EncodingCompleter])]
        [Encoding] $Encoding = [UTF8Encoding]::new(),

        [Parameter(ParameterSetName = 'Path')]
        [Parameter(ParameterSetName = 'LiteralPath')]
        [switch] $Raw,

        [Parameter(ParameterSetName = 'PathToFile')]
        [Parameter(ParameterSetName = 'LiteralPathToFile')]
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
            if(-not $IsCoreCLR) {
                return $items | GzipFrameworkReader @params
            }

            $items | GzipCoreReader @params
        }
        catch {
            $PSCmdlet.WriteError($_)
        }
        finally {
            if($params['OutStream'] -is [IDisposable]) {
                $params['OutStream'].Dispose()
            }

            if($PassThru.IsPresent) {
                $outFile.Name -as [FileInfo]
            }
        }
    }
}
