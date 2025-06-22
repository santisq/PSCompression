using namespace System.IO

$moduleName = (Get-Item ([Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'ZipEntryDirectory Class' {
    BeforeAll {
        $zip = New-Item (Join-Path $TestDrive test.zip) -ItemType File -Force
        New-ZipEntry $zip.FullName -EntryPath afolder/
    }

    It 'Should be of type Directory' {
        ($zip | Get-ZipEntry).Type | Should -BeExactly ([PSCompression.EntryType]::Directory)
    }
}
