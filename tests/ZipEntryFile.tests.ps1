$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'ZipEntryFile Class' {
    BeforeAll {
        $zip = New-Item (Join-Path $TestDrive test.zip) -ItemType File -Force
        'hello world!' | New-ZipEntry $zip.FullName -EntryPath helloworld.txt
    }

    It 'Should be of type Archive' {
        ($zip | Get-ZipEntry).Type | Should -BeExactly ([PSCompression.EntryType]::Archive)
    }

    It 'Should Have a BaseName Property' {
        ($zip | Get-ZipEntry).BaseName | Should -BeOfType ([string])
        ($zip | Get-ZipEntry).BaseName | Should -BeExactly helloworld
    }

    It 'Should Have an Extension Property' {
        ($zip | Get-ZipEntry).Extension | Should -BeOfType ([string])
        ($zip | Get-ZipEntry).Extension | Should -BeExactly .txt
    }

    It 'Should Open the source zip' {
        Use-Object ($stream = ($zip | Get-ZipEntry).OpenRead()) {
            $stream | Should -BeOfType ([System.IO.Compression.ZipArchive])
        }
    }
}
