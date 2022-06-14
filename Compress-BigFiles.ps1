using namespace System.IO
using namespace System.IO.Compression

Add-Type -AssemblyName System.IO.Compression

function Compress-BigFiles {
    [CmdletBinding()]
    param(
        [parameter(Mandatory, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias('FullName')]
        [ValidateScript({
            if($PSCmdlet.GetUnresolvedProviderPathFromPSPath($_) | Test-Path) {
                return $true
            }

            throw [InvalidOperationException]::new(
                "The path '$_' either does not exist or is not a valid file system path."
            )
        })]
        [string] $Path,

        [parameter(Mandatory)]
        [ValidateScript({
            if($PSCmdlet.GetUnresolvedProviderPathFromPSPath($_) | Split-Path | Test-Path) {
                return $true
            }

            throw [InvalidOperationException]::new(
                "The path '$_' either does not exist or is not a valid file system path."
            )
        })]
        [string] $DestinationPath,

        [parameter()]
        [CompressionLevel] $CompressionLevel = [CompressionLevel]::Optimal
    )

    begin {
        $DestinationPath = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($DestinationPath)
        $Path   = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($Path)
        $destfs = [File]::Create($DestinationPath)
        $zip    = [ZipArchive]::new($destfs, [ZipArchiveMode]::Create)
        $here   = $pwd.Path
    }
    process {
        foreach($file in Get-ChildItem $Path -File -Recurse) {
            try {
                $relative = $file.FullName.Substring($here.Length + 1).Replace('\', '/')
                $sourcefs = $file.Open([FileMode]::Open, [FileAccess]::Read, [FileShare]::Read)
                $entry    = $zip.CreateEntry($relative, $CompressionLevel)
                $entryfs  = $entry.Open()
                $sourcefs.CopyTo($entryfs)
            }
            catch {
                $PSCmdlet.WriteError($_)
            }
            finally {
                ($entryfs, $sourcefs).ForEach('Dispose')
            }
        }
    }
    end {
        ($zip, $destfs).ForEach('Dispose')
    }
}