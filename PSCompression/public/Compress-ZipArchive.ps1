using namespace System.IO
using namespace System.IO.Compression
using namespace System.Collections.Generic

function Compress-ZipArchive {
    <#
    .SYNOPSIS
    PowerShell function that overcomes the limitations of `Compress-Archive` while keeping similar pipeline capabilities.

    .DESCRIPTION
    `Compress-ZipArchive` intends to solve the present limitations of the built-in cmdlet `Compress-Archive`:
        1. Compression is limited to files below 2Gb
        2. It cannot compress files in use by another process, even though explorer can.

    .Parameter Path
    Specifies the path or paths to the files that you want to add to the archive zipped file.
    To specify multiple paths, and include files in multiple locations, use commas to separate the paths.
    This Parameter accepts wildcard characters. Wildcard characters allow you to add all files in a directory to your archive file.

    .Parameter LiteralPath
    Specifies the path or paths to the files that you want to add to the archive zipped file.
    Unlike the Path Parameter, the value of LiteralPath is used exactly as it's typed.
    No characters are interpreted as wildcards

    .Parameter DestinationPath
    The destination path to the Zip file.
    If the file name in DestinationPath doesn't have a `.zip` file name extension, the function appends the `.zip` file name extension.

    .Parameter CompressionLevel
    Define the compression level that should be used.
    See https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.compressionlevel for details.

    .Parameter Update
    Updates Zip entries and adds new entries to an existing Zip file.

    .Parameter Force
    Replaces an existing Zip file with a new one. All Zip contents will be lost.

    .Parameter PassThru
    Outputs the object representing the compressed file. The function produces no output by default.

    .EXAMPLE
    Get-ChildItem .\path -Recurse -Filter *.ext |
        Compress-ZipArchive -DestinationPath dest.zip

    Compress all `.ext` files from a specific folder.

    .EXAMPLE
    Compress-ZipArchive .\*\*.txt -DestinationPath dest.zip

    Compress all `.txt` files contained in all folders in the Current Directory.

    .EXAMPLE
    Get-ChildItem .\path -Recurse -Include *.ext, *.ext2 |
        Compress-ZipArchive -DestinationPath dest.zip

    Compress all `.ext` and `.ext2` from a specific folder.

    .EXAMPLE
    Compress-ZipArchive .\path -Destination myPath.zip -CompressionLevel Fastest

    Compress a folder using "Fastest" Compression Level.

    .EXAMPLE
    Get-ChildItem .\path -Recurse -Directory |
        Compress-ZipArchive -DestinationPath dest.zip

    Compressing all directories in ".\Path".

    .EXAMPLE
    Compress-ZipArchive -Path .\path -DestinationPath dest.zip -Force

    Replacing an existing Zip Archive.

    .EXAMPLE
    Get-ChildItem .\path -Recurse -Directory |
        Compress-ZipArchive -DestinationPath dest.zip -Update

    Adding and updating new entries to an existing Zip Archive.

    .LINK
    https://github.com/santisq/PSCompression
    #>

    [CmdletBinding(DefaultParameterSetName = 'Path')]
    [Alias('zip', 'ziparchive')]
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
        $isLiteral  = $false
        $targetPath = $Path

        if($PSBoundParameters.ContainsKey('LiteralPath')) {
            $isLiteral  = $true
            $targetPath = $LiteralPath
        }

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

        $queue = [Queue[FileSystemInfo]]::new()

        foreach($item in $ExecutionContext.InvokeProvider.Item.Get($targetPath, $true, $isLiteral)) {
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

                        $sourcefs = $item.Open([FileMode]::Open, [FileAccess]::Read, [FileShare] 'ReadWrite, Delete')
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
        }
    }
    end {
        $zip, $destfs | ForEach-Object Dispose

        if($PassThru.IsPresent) {
            $DestinationPath -as [FileInfo]
        }
    }
}