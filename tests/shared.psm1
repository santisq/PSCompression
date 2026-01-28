using namespace System.Management.Automation
using namespace System.Runtime.InteropServices
using namespace PSCompression

function Complete-Input {
    param([string] $Expression)

    [CommandCompletion]::CompleteInput($Expression, $Expression.Length, $null).CompletionMatches
}

function Test-Completer {
    param(
        [ArgumentCompleter([EncodingCompleter])]
        [string] $Test
    )
}

function Get-Structure {
    foreach ($folder in 0..5) {
        $folder = 'testfolder{0:D2}/' -f $folder
        $folder
        foreach ($file in 0..5) {
            [System.IO.Path]::Combine($folder, 'testfile{0:D2}.txt' -f $file)
        }
    }
}

function New-Structure {
    param(
        [Parameter(ValueFromPipeline, Mandatory)]
        [string] $Item,

        [Parameter(Mandatory, Position = 0)]
        [string] $Path)

    begin {
        $fileCount = $dirCount = 0
    }
    process {
        $isFile = $Item.EndsWith('.txt')

        $newItemSplat = @{
            ItemType = ('Directory', 'File')[$isFile]
            Value    = Get-Random
            Force    = $true
            Path     = Join-Path $Path $Item
        }

        $null = New-Item @newItemSplat

        if ($isFile) {
            $fileCount++
            return
        }

        $dirCount++
    }
    end {
        $dirCount++ # Includes the folder itself

        [pscustomobject]@{
            File      = $fileCount
            Directory = $dirCount
        }
    }
}

$osIsWindows = [RuntimeInformation]::IsOSPlatform([OSPlatform]::Windows)
$osIsWindows | Out-Null

$exportModuleMemberSplat = @{
    Variable = 'moduleName', 'manifestPath', 'osIsWindows'
    Function = 'Decode', 'Complete', 'Test-Completer', 'Get-Structure', 'New-Structure'
}

Export-ModuleMember @exportModuleMemberSplat
