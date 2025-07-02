using namespace System.IO
using namespace System.Reflection
using namespace PSCompression.Shared

$moduleName = [Path]::GetFileNameWithoutExtension($PSCommandPath)
$frame = 'net8.0'

if (-not $IsCoreCLR) {
    $frame = 'netstandard2.0'
    $asm = [Path]::Combine($PSScriptRoot, 'bin', $frame, "${moduleName}.dll")
    Import-Module -Name $asm -ErrorAction Stop -PassThru
    return
}

$context = [Path]::Combine($PSScriptRoot, 'bin', $frame, "${moduleName}.Shared.dll")
$isReload = $true

if (-not ("${moduleName}.Shared.LoadContext" -as [type])) {
    $isReload = $false
    Add-Type -Path $context
}

$mainModule = [LoadContext]::Initialize()
$innerMod = Import-Module -Assembly $mainModule -PassThru:$isReload

if ($innerMod) {
    $addExportedCmdlet = [psmoduleinfo].GetMethod(
        'AddExportedCmdlet', [BindingFlags] 'Instance, NonPublic')

    foreach ($cmd in $innerMod.ExportedCmdlets.Values) {
        $addExportedCmdlet.Invoke($ExecutionContext.SessionState.Module, @($cmd))
    }
}
