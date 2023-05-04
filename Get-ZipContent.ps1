using namespace System.IO.Compression

function Get-ZipContent {
    [CmdletBinding()]
    param(
        [Parameter(ParameterSetName = 'Path', Position = 0, Mandatory, ValueFromPipeline)]
        [string[]] $Path,

        [Parameter(ParameterSetName = 'LiteralPath', Mandatory, ValueFromPipelineByPropertyName)]
        [Alias('PSPath')]
        [string[]] $LiteralPath,

        [Parameter()]
        [switch] $Force
    )

    process {
        try {
            $arguments = switch($PSCmdlet.ParameterSetName) {
                Path { $Path, $Force.IsPresent, $false }
                LiteralPath { $LiteralPath, $Force.IsPresent, $true }
            }

            foreach($item in $PSCmdlet.InvokeProvider.Item.Get.Invoke($arguments)) {
                try {
                    $fs = $item.OpenRead()
                    $zip = [ZipArchive]::new($fs, [ZipArchiveMode]::Read)
                    foreach($entry in $zip.Entries) {
                        $entry.PSObject.Properties.Add([psnoteproperty]::new('Source', $item.FullName))
                        $entry
                    }
                }
                catch {
                    $PSCmdlet.WriteError($_)
                }
                finally {
                    if($zip -is [IDisposable]) {
                        $zip.Dispose()
                    }

                    if($fs -is [IDisposable]) {
                        $fs.Dispose()
                    }
                }
            }
        }
        catch {
            $PSCmdlet.WriteError($_)
        }
    }
}
