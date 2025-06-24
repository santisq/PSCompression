using namespace System.IO

$moduleName = (Get-Item ([Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'Directory Entry Types' {
    BeforeAll {
        $zip = New-Item (Join-Path $TestDrive test.zip) -ItemType File -Force
        New-ZipEntry $zip.FullName -EntryPath afolder/
        $tarArchive = New-Item (Join-Path $TestDrive afolder) -ItemType Directory -Force |
            Compress-TarArchive -Destination 'testTarFile' -PassThru

        $tarArchive | Out-Null
    }

    It 'Should be of type Directory' {
        ($zip | Get-ZipEntry).Type | Should -BeExactly ([PSCompression.EntryType]::Directory)
        ($tarArchive | Get-TarEntry).Type | Should -BeExactly ([PSCompression.EntryType]::Directory)
    }
}
