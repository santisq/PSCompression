using namespace System.IO
using namespace System.IO.Compression

function Expand-ZipArchive {
    [CmdletBinding(DefaultParameterSetName = 'Path')]
    param(
        [Parameter(ParameterSetName = 'Path', Mandatory, Position = 0, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [string] $Path,

        [Parameter(ParameterSetName = 'LiteralPath', Mandatory, ValueFromPipelineByPropertyName)]
        [Alias('PSPath')]
        [string] $LiteralPath,

        [Parameter(Mandatory, Position = 1)]
        [string] $DestinationPath,

        [Parameter()]
        [switch] $PassThru,

        [Parameter()]
        [switch] $Force
    )

    begin {
        $DestinationPath = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($DestinationPath)
        $fsMode = [FileMode]::CreateNew
        if($Force.IsPresent) {
            $fsMode = [FileMode]::Create
        }
    }
    process {
        $arguments = switch($PSCmdlet.ParameterSetName) {
            Path { $Path, $false, $false }
            LiteralPath { $LiteralPath, $false, $true }
        }

        foreach($item in $PSCmdlet.InvokeProvider.Item.Get.Invoke($arguments)) {
            try {
                $fileStream = $item.OpenRead()
                $zipArchive = [ZipArchive]::new($fileStream, [ZipArchiveMode]::Read)
                foreach($entry in $zipArchive.Entries) {
                    $destPath = [Path]::GetFullPath([Path]::Combine($DestinationPath, $entry.FullName))

                    # if it's a folder, create it and go next
                    if(-not $entry.Name) {
                        $null = [Directory]::CreateDirectory($destPath)
                        continue
                    }

                    $destParent = [Path]::GetDirectoryName($destPath)

                    if(-not [Path]::Exists($destParent)) {
                        $null = [Directory]::CreateDirectory($destParent)
                    }

                    try {
                        $childStream = [FileStream]::new($destPath, $fsMode)
                        $wrappedStream = $entry.Open()
                        $wrappedStream.CopyTo($childStream)

                        if($PassThru.IsPresent) {
                            $childStream.Name -as [FileInfo]
                        }
                    }
                    catch {
                        $PSCmdlet.WriteError($_)
                    }
                    finally {
                        if($childStream) {
                            $childStream.Dispose()
                        }

                        if($wrappedStream) {
                            $wrappedStream.Dispose()
                        }
                    }
                }
            }
            catch {
                $PSCmdlet.WriteError($_)
            }
            finally {
                if($zipArchive) {
                    $zipArchive.Dispose()
                }

                if($fileStream) {
                    $fileStream.Dispose()
                }
            }
        }
    }
}
