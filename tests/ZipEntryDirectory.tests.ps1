$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'ZipEntryDirectory Class' {
    BeforeAll {
        $zip = New-Item (Join-Path $TestDrive test.zip) -ItemType File -Force
        New-ZipEntry $zip.FullName -EntryPath afolder/
    }

    It 'Should be of type Directory' {
        ($zip | Get-ZipEntry).Type | Should -BeExactly ([PSCompression.ZipEntryType]::Directory)
    }
}
