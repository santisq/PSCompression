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
        $zipEntry = 'hello world!' | New-ZipEntry $zip.FullName -EntryPath helloworld.txt
        $tarEntry = New-Item (Join-Path $TestDrive helloworld.txt) -ItemType File -Force |
            Compress-TarArchive -Destination 'testTarDirectory' -PassThru |
            Get-TarEntry

        $tarArchive, $zipEntry, $tarEntry | Out-Null
    }

    It 'Should be of type Archive' {
        $zipEntry.Type | Should -BeExactly ([PSCompression.Enum.EntryType]::Archive)
        $tarEntry.Type | Should -BeExactly ([PSCompression.Enum.EntryType]::Archive)
    }

    It 'Should Have a BaseName Property' {
        $zipEntry.BaseName | Should -BeOfType ([string])
        $zipEntry.BaseName | Should -BeExactly helloworld

        $tarEntry.BaseName | Should -BeOfType ([string])
        $tarEntry.BaseName | Should -BeExactly helloworld
    }

    It 'Should Have an Extension Property' {
        $zipEntry.Extension | Should -BeOfType ([string])
        $zipEntry.Extension | Should -BeExactly .txt

        $tarEntry.Extension | Should -BeOfType ([string])
        $tarEntry.Extension | Should -BeExactly .txt
    }

    It 'Should Have an IsEncrypted Property' {
        $zipEntry.IsEncrypted | Should -BeOfType ([bool])
        $zipEntry.IsEncrypted | Should -BeFalse
    }

    It 'Should Have an AESKeySize Property' {
        $zipEntry.AESKeySize | Should -BeOfType ([int])
        $zipEntry.AESKeySize | Should -BeExactly 0
    }

    It 'Should Have a CompressionMethod Property' {
        $zipEntry.CompressionMethod | Should -Be Deflated
    }

    It 'Should Have a Comment Property' {
        $zipEntry.Comment | Should -BeOfType ([string])
        $zipEntry.Comment | Should -BeExactly ''
    }

    It 'Should Open the source zip' {
        Use-Object ($stream = $zipEntry.OpenRead()) {
            $stream | Should -BeOfType ([ZipArchive])
        }
    }
}
