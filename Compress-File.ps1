using namespace System.IO
using namespace System.IO.Compression
using namespace System.Collections.Generic

Add-Type -AssemblyName System.IO.Compression

function Compress-File {
    [CmdletBinding(DefaultParameterSetName = 'Force')]
    param(
        [parameter(Position = 0, Mandatory, ValueFromPipeline, ValueFromPipelineByPropertyName)]
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

        [parameter(Position = 1, Mandatory)]
        [string] $DestinationPath,

        [parameter()]
        [CompressionLevel] $CompressionLevel = [CompressionLevel]::Optimal,

        [parameter(ParameterSetName = 'Update')]
        [switch] $Update,

        [parameter(ParameterSetName = 'Force')]
        [switch] $Force,

        [parameter()]
        [switch] $PassThru
    )

    begin {
        $DestinationPath = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($DestinationPath)
        $Path = $PSCmdlet.GetUnresolvedProviderPathFromPSPath($Path)

        if($Force.IsPresent) {
            $fsMode = [FileMode]::Create
        }
        elseif($Update.IsPresent) {
            $fsMode = [FileMode]::OpenOrCreate
        }
        else {
            $fsMode = [FileMode]::CreateNew
        }

        $ExpectingInput = $null
    }
    process {
        if(-not $ExpectingInput) {
            try {
                $destfs = [File]::Open($DestinationPath, $fsMode)
                $zip    = [ZipArchive]::new($destfs, [ZipArchiveMode]::Update)
                $ExpectingInput = $true
            }
            catch {
                $zip, $destfs | ForEach-Object Dispose
                $PSCmdlet.ThrowTerminatingError($_)
            }
        }

        if([File]::GetAttributes($Path) -band [FileAttributes]::Archive) {
            [FileInfo] $Path = $Path
            $here = $Path.Directory.FullName
        }
        else {
            [DirectoryInfo] $Path = $Path
            $here = $Path.Parent.FullName
        }

        $queue = [Queue[FileSystemInfo]]::new()
        $queue.Enqueue($Path)

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
                    $entry    = $zip.GetEntry($relative)

                    if($item -is [DirectoryInfo]) {
                        $queue.Enqueue($item)
                        if(-not $entry) {
                            $entry = $zip.CreateEntry($relative + '\', $CompressionLevel)
                        }
                        continue
                    }

                    if(-not $entry) {
                        $entry = $zip.CreateEntry($relative, $CompressionLevel)
                    }

                    $sourcefs = $item.Open([FileMode]::Open, [FileAccess]::Read, [FileShare]::Read)
                    $entryfs  = $entry.Open()
                    $sourcefs.CopyTo($entryfs)
                }
                catch {
                    $PSCmdlet.WriteError($_)
                }
                finally {
                    $entryfs, $sourcefs | ForEach-Object Dispose
                }
            }
        }

        if($PassThru.IsPresent) { $Path }
    }
    end {
        $zip, $destfs | ForEach-Object Dispose
    }
}