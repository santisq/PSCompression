using namespace System.IO
using namespace System.IO.Compression

Add-Type -AssemblyName System.IO.Compression

function Compress-BigFiles {
    [CmdletBinding(DefaultParameterSetName = 'Force')]
    param(
        [parameter(Mandatory, ValueFromPipeline, ValueFromPipelineByPropertyName)]
        [Alias('FullName')]
        [ValidateScript({
            if(Test-Path -LiteralPath $_) {
                return $true
            }

            throw [InvalidOperationException]::new(
                "The path '$_' either does not exist or is not a valid file system path."
            )
        })]
        [string] $Path,

        [parameter(Mandatory)]
        [string] $DestinationPath,

        [parameter()]
        [CompressionLevel] $CompressionLevel = [CompressionLevel]::Optimal,

        [parameter(ParameterSetName = 'Update')]
        [switch] $Update,

        [parameter(ParameterSetName = 'Force')]
        [switch] $Force
    )

    begin {
        $DestinationPath = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($DestinationPath)
        $here = $Path = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($Path)
        if(Test-Path $here -PathType Leaf) {
            $here = Split-Path $here
        }

        if($Force.IsPresent) {
            $fsMode = [FileMode]::Create
        }
        elseif($Update.IsPresent) {
            $fsMode = [FileMode]::OpenOrCreate
        }
        else {
            $fsMode = [FileMode]::CreateNew
        }

        try {
            $destfs = [File]::Open($DestinationPath, $fsMode)
            $zip    = [ZipArchive]::new($destfs, [ZipArchiveMode]::Update)
        }
        catch {
            $PSCmdlet.ThrowTerminatingError($_)
        }
    }
    process {
        foreach($file in Get-ChildItem $Path -File -Recurse -Force) {
            try {
                $relative = $file.FullName.Substring($here.Length + 1).Replace('\', '/')
                $sourcefs = $file.Open([FileMode]::Open, [FileAccess]::Read, [FileShare]::Read)
                $entry    = $zip.GetEntry($relative)
                if(-not $entry) {
                    $entry = $zip.CreateEntry($relative, $CompressionLevel)
                }
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