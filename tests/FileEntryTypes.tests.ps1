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
            Compress-TarArchive -Destination 'testTarDirectory' -PassThru

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

    It 'Should Have an IsEncrypted Property' {
        ($zip | Get-ZipEntry).IsEncrypted | Should -BeOfType ([bool])
        ($zip | Get-ZipEntry).IsEncrypted | Should -BeFalse
    }

    It 'Should Have an AESKeySize Property' {
        ($zip | Get-ZipEntry).AESKeySize | Should -BeOfType ([int])
        ($zip | Get-ZipEntry).AESKeySize | Should -BeExactly 0
    }

    It 'Should Have a CompressionMethod Property' {
        ($zip | Get-ZipEntry).CompressionMethod | Should -Be Deflated
    }

    It 'Should Have a Comment Property' {
        ($zip | Get-ZipEntry).Comment | Should -BeOfType ([string])
        ($zip | Get-ZipEntry).Comment | Should -BeExactly ''
    }

    It 'Should Open the source zip' {
        Use-Object ($stream = ($zip | Get-ZipEntry).OpenRead()) {
            $stream | Should -BeOfType ([ZipArchive])
        }
    }
}
