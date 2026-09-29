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
        $rarEntry = Get-RarEntry $PSScriptRoot/../assets/test.rar -Include *.txt
        $tarEntry = New-Item (Join-Path $TestDrive helloworld.txt) -ItemType File -Force |
            Compress-TarArchive -Destination 'testTarDirectory' -PassThru |
            Get-TarEntry
        $tarArchive, $zipEntry, $tarEntry, $rarEntry | Out-Null
    }

    It 'Should be of type Archive' {
        $zipEntry.Type | Should -BeExactly ([PSCompression.Enum.EntryType]::Archive)
        $tarEntry.Type | Should -BeExactly ([PSCompression.Enum.EntryType]::Archive)
        $rarEntry.Type | Should -BeExactly ([PSCompression.Enum.EntryType]::Archive)
    }

    It 'Should have a BaseName property' {
        $zipEntry.BaseName | Should -BeExactly helloworld
        $rarEntry.BaseName | Should -BeExactly test
        $tarEntry.BaseName | Should -BeExactly helloworld
    }

    It 'Should have an Extension property' {
        $zipEntry.Extension | Should -BeExactly .txt
        $tarEntry.Extension | Should -BeExactly .txt
        $rarEntry.Extension | Should -BeExactly .txt
    }

    It 'Should have an IsEncrypted property' {
        $zipEntry.IsEncrypted | Should -BeFalse
        $rarEntry.IsEncrypted | Should -BeFalse
    }

    It 'Should have an AESKeySize property' {
        $zipEntry.AESKeySize | Should -BeOfType ([int])
        $zipEntry.AESKeySize | Should -BeExactly 0
    }

    It 'Should have a CompressionMethod property' {
        $zipEntry.CompressionMethod | Should -Be Deflated
    }

    It 'Should have a CompressionRatio property' {
        $zipEntry.CompressionRatio | Should -BeOfType ([string])
        $rarEntry.CompressionRatio | Should -BeOfType ([string])
    }

    It 'Should have a LastWriteTime property' {
        $zipEntry.LastWriteTime | Should -BeOfType ([datetime])
        $rarEntry.LastWriteTime | Should -BeOfType ([datetime])
        $tarEntry.LastWriteTime | Should -BeOfType ([datetime])
    }

    It 'Should have a Comment property' {
        $zipEntry.Comment | Should -BeOfType ([string])
        $zipEntry.Comment | Should -BeExactly ''
    }

    It 'Should Open the source zip' {
        Use-Object ($stream = $zipEntry.OpenRead()) {
            $stream | Should -BeOfType ([ZipArchive])
        }
    }
}
