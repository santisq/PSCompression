using namespace System.Management.Automation
using namespace System.Runtime.InteropServices
using namespace PSCompression

function Complete {
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

$osIsWindows = [RuntimeInformation]::IsOSPlatform([OSPlatform]::Windows)
$osIsWindows | Out-Null

$exportModuleMemberSplat = @{
    Variable = 'moduleName', 'manifestPath', 'osIsWindows'
    Function = 'Decode', 'Complete', 'Test-Completer', 'Get-Structure'
}

Export-ModuleMember @exportModuleMemberSplat
