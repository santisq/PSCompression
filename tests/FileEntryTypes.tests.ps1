using namespace System.IO
using namespace System.IO.Compression

$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'File Entry Types' {
    BeforeAll {
        $zip = New-Item (Join-Path $TestDrive test.zip) -ItemType File -Force
        'hello world!' | New-ZipEntry $zip.FullName -EntryPath helloworld.txt

        $tarArchive = New-Item (Join-Path $TestDrive helloworld.txt) -ItemType File -Force |
            Compress-TarArchive -Destination 'testTarFile' -PassThru

        $tarArchive | Out-Null
    }

    It 'Should be of type Archive' {
        ($zip | Get-ZipEntry).Type | Should -BeExactly ([PSCompression.EntryType]::Archive)

        ($tarArchive | Get-TarEntry).Type | Should -BeExactly ([PSCompression.EntryType]::Archive)
    }

    It 'Should Have a BaseName Property' {
        ($zip | Get-ZipEntry).BaseName | Should -BeOfType ([string])
        ($zip | Get-ZipEntry).BaseName | Should -BeExactly helloworld

        ($tarArchive | Get-TarEntry).BaseName | Should -BeOfType ([string])
        ($tarArchive | Get-TarEntry).BaseName | Should -BeExactly helloworld
    }

    It 'Should Have an Extension Property' {
        ($zip | Get-ZipEntry).Extension | Should -BeOfType ([string])
        ($zip | Get-ZipEntry).Extension | Should -BeExactly .txt

        ($tarArchive | Get-TarEntry).Extension | Should -BeOfType ([string])
        ($tarArchive | Get-TarEntry).Extension | Should -BeExactly .txt
    }

    It 'Should Open the source zip' {
        Use-Object ($stream = ($zip | Get-ZipEntry).OpenRead()) {
            $stream | Should -BeOfType ([ZipArchive])
        }
    }
}
