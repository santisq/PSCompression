using namespace System.IO
using namespace System.IO.Compression

$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'ZipEntryBase Class' {
    BeforeAll {
        $zip = New-Item (Join-Path $TestDrive test.zip) -ItemType File -Force
        'hello world!' | New-ZipEntry $zip.FullName -EntryPath helloworld.txt
        New-ZipEntry $zip.FullName -EntryPath somefolder/
        $encryptedZip = Get-Item $PSScriptRoot/../assets/helloworld.zip
        $encryptedZip | Out-Null
    }

    It 'Can extract an entry' {
        ($zip | Get-ZipEntry -Type Archive).ExtractTo($TestDrive, $false) |
            Should -BeOfType ([FileInfo])
    }

    It 'Can overwrite a file when extracting' {
        ($zip | Get-ZipEntry -Type Archive).ExtractTo($TestDrive, $true) |
            Should -BeOfType ([FileInfo])
    }

    It 'Can extract a file from entries created from input Stream' {
        Use-Object ($stream = $zip.OpenRead()) {
            ($stream | Get-ZipEntry -Type Archive).ExtractTo($TestDrive, $true)
        } | Should -BeOfType ([FileInfo])
    }

    It 'Can create a new folder in the destination path when extracting' {
        $entry = $zip | Get-ZipEntry -Type Archive
        $file = $entry.ExtractTo([Path]::Combine($TestDrive, 'myTestFolder'), $false)

        $file.FullName | Should -BeExactly ([Path]::Combine($TestDrive, 'myTestFolder', $entry.Name))
    }

    It 'Can extract an encrypted entry' {
        $passw = ConvertTo-SecureString 'test' -AsPlainText -Force
        $dest = Join-Path $TestDrive encryptedTestFolder
        Use-Object ($stream = $encryptedZip.OpenRead()) {
            $info = ($stream | Get-ZipEntry -Type Archive).ExtractTo($dest, $false, $passw)
            $info | Should -BeOfType ([FileInfo])
            Get-Content $info.FullName | Should -BeExactly 'hello world!'
            $info.Delete()
        }

        $info = ($encryptedZip | Get-ZipEntry).ExtractTo($dest, $false, $passw)
        $info | Should -BeOfType ([FileInfo])
        Get-Content $info.FullName | Should -BeExactly 'hello world!'
        $info.Delete()
    }

    It 'Can extract folders' {
        ($zip | Get-ZipEntry -Type Directory).ExtractTo($TestDrive, $false) |
            Should -BeOfType ([DirectoryInfo])
    }

    It 'Can overwrite folders when extracting' {
        ($zip | Get-ZipEntry -Type Directory).ExtractTo($TestDrive, $true) |
            Should -BeOfType ([DirectoryInfo])
    }

    It 'Has a LastWriteTime Property' {
        ($zip | Get-ZipEntry).LastWriteTime | Should -BeOfType ([datetime])
    }

    It 'Has a CompressionRatio Property' {
        New-ZipEntry $zip.FullName -EntryPath empty.txt
        ($zip | Get-ZipEntry).CompressionRatio | Should -BeOfType ([string])
    }

    It 'Can remove an entry in the source zip' {
        { $zip | Get-ZipEntry | ForEach-Object Remove } |
            Should -Not -Throw

        $zip | Get-ZipEntry | Should -BeNullOrEmpty
    }

    It 'Should throw if Remove() is used on entries created from input Stream' {
        'hello world!' | New-ZipEntry $zip.FullName -EntryPath helloworld.txt

        {
            Use-Object ($stream = $zip.OpenRead()) {
                $stream | Get-ZipEntry -Type Archive | ForEach-Object Remove
            }
        } | Should -Throw
    }

    It 'Opens a ZipArchive on OpenRead() and OpenWrite()' {
        Use-Object ($archive = ($zip | Get-ZipEntry).OpenRead()) {
            $archive | Should -BeOfType ([ZipArchive])
        }

        Use-Object ($stream = $zip.OpenRead()) {
            Use-Object ($archive = ($stream | Get-ZipEntry).OpenRead()) {
                $archive | Should -BeOfType ([ZipArchive])
            }
        }

        Use-Object ($archive = ($zip | Get-ZipEntry).OpenWrite()) {
            $archive | Should -BeOfType ([ZipArchive])
        }
    }

    It 'Should throw if calling OpenWrite() on entries created from input Stream' {
        Use-Object ($stream = $zip.OpenRead()) {
            {
                Use-Object ($stream | Get-ZipEntry).OpenWrite() { }
            } | Should -Throw
        }
    }

    It 'Should throw if the entry to extract no longer exists in the source zip' {
        $entry = New-ZipEntry $zip.FullName -EntryPath toberemoved.txt
        $entry | Remove-ZipEntry
        {
            $entry.ExtractTo($TestDrive, $false)
        } | Should -Throw
    }
}
