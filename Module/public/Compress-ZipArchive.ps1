using namespace System.IO
using namespace System.IO.Compression
using namespace System.Collections.Generic

# .ExternalHelp PSCompression-help.xml
function Compress-ZipArchive {
    [CmdletBinding(DefaultParameterSetName = 'Path')]
    [Alias('zip', 'ziparchive')]
    [OutputType([System.IO.FileInfo])]
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

        [Parameter(Position = 1, Mandatory)]
        [string] $DestinationPath,

        [Parameter()]
        [CompressionLevel] $CompressionLevel = [CompressionLevel]::Optimal,

        [Parameter(ParameterSetName = 'PathWithUpdate', Mandatory)]
        [Parameter(ParameterSetName = 'LiteralPathWithUpdate', Mandatory)]
        [switch] $Update,

        [Parameter(ParameterSetName = 'PathWithForce', Mandatory)]
        [Parameter(ParameterSetName = 'LiteralPathWithForce', Mandatory)]
        [switch] $Force,

        [Parameter()]
        [switch] $PassThru
    )

    begin {
        $DestinationPath = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($DestinationPath)

        if([Path]::GetExtension($DestinationPath) -ne '.zip') {
            $DestinationPath = $DestinationPath + '.zip'
        }

        $zipMode = [ZipArchiveMode]::Update

        if($Force.IsPresent) {
            $fsMode = [FileMode]::Create
        }
        elseif($Update.IsPresent) {
            $fsMode = [FileMode]::OpenOrCreate
        }
        else {
            $fsMode = [FileMode]::CreateNew
            $zipMode = [ZipArchiveMode]::Create
        }

        $ExpectingInput = $null
    }
    process {
        $isLiteral = $false
        $targetPath = $Path

        if($PSBoundParameters.ContainsKey('LiteralPath')) {
            $isLiteral = $true
            $targetPath = $LiteralPath
        }

        if(-not $ExpectingInput) {
            try {
                $null = [Directory]::CreateDirectory([Path]::GetDirectoryName($DestinationPath))
                $destfs = [File]::Open($DestinationPath, $fsMode)
                $zip = [ZipArchive]::new($destfs, $zipMode)
                $ExpectingInput = $true
            }
            catch {
                $zip, $destfs | ForEach-Object Dispose
                $PSCmdlet.ThrowTerminatingError($_)
            }
        }

        $queue = [Queue[FileSystemInfo]]::new()

        foreach($item in $PSCmdlet.InvokeProvider.Item.Get($targetPath, $true, $isLiteral)) {
            $queue.Enqueue($item)

            $here = $item.Parent.FullName
            if($item -is [FileInfo]) {
                $here = $item.Directory.FullName
            }

            while($queue.Count) {
                try {
                    $current = $queue.Dequeue()
                    if($current -is [DirectoryInfo]) {
                        $current = $current.EnumerateFileSystemInfos()
                    }
                }
                catch {
                    $PSCmdlet.WriteError($_)
                    continue
                }

                foreach($item in $current) {
                    try {
                        if($item.FullName -eq $DestinationPath) {
                            continue
                        }

                        $relative = $item.FullName.Substring($here.Length + 1)

                        if($Update.IsPresent) {
                            $entry = $zip.GetEntry($relative)
                        }

                        if($item -is [DirectoryInfo]) {
                            $queue.Enqueue($item)
                            if(-not $Update.IsPresent -or -not $entry) {
                                $entry = $zip.CreateEntry($relative + '\', $CompressionLevel)
                            }
                            continue
                        }

                        if(-not $Update.IsPresent -or -not $entry) {
                            $entry = $zip.CreateEntry($relative, $CompressionLevel)
                        }

                        $sourcefs = $item.Open([FileMode]::Open, [FileAccess]::Read, [FileShare] 'ReadWrite, Delete')
                        $entryfs = $entry.Open()
                        $sourcefs.CopyTo($entryfs)
                    }
                    catch {
                        $PSCmdlet.WriteError($_)
                    }
                    finally {
                        if($entryfs -is [System.IDisposable]) {
                            $entryfs.Dispose()
                        }
                        if($sourcefs -is [System.IDisposable]) {
                            $sourcefs.Dispose()
                        }
                    }
                }
            }
        }
    }
    end {
        if($zip -is [System.IDisposable]) {
            $zip.Dispose()
        }

        if($destfs -is [System.IDisposable]) {
            $destfs.Dispose()
        }

        if($PassThru.IsPresent) {
            $DestinationPath -as [FileInfo]
        }
    }
}
