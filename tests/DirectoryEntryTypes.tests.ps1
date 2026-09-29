using namespace System.IO

$moduleName = (Get-Item ([Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'Directory Entry Types' {
    BeforeAll {
        $zip = New-Item (Join-Path $TestDrive test.zip) -ItemType File -Force
        $zipEntry = New-ZipEntry $zip.FullName -EntryPath afolder/
        $tarEntry = New-Item (Join-Path $TestDrive afolder) -ItemType Directory -Force |
            Compress-TarArchive -Destination 'testTarFile' -PassThru |
            Get-TarEntry

        $zipEntry, $tarEntry | Out-Null
    }

    It 'Should be of type Directory' {
        $zipEntry.Type | Should -BeExactly ([PSCompression.Enum.EntryType]::Directory)
        $tarEntry.Type | Should -BeExactly ([PSCompression.Enum.EntryType]::Directory)
    }
}
