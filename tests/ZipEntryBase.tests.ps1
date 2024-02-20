$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'ZipEntryBase Class' {
    BeforeAll {
        $zip = New-Item (Join-Path $TestDrive test.zip) -ItemType File -Force
        'hello world!' | New-ZipEntry $zip.FullName -EntryPath helloworld.txt
    }

    It 'Can extract an entry' {
        ($zip | Get-ZipEntry).ExtractTo($TestDrive, $false) |
            Should -BeOfType ([System.IO.FileInfo])
    }

    It 'Can overwrite a file when extracting' {
        ($zip | Get-ZipEntry).ExtractTo($TestDrive, $true) |
            Should -BeOfType ([System.IO.FileInfo])
    }

    It 'Can create a new folder in the destination path when extracting' {
        $entry = $zip | Get-ZipEntry
        $file = $entry.ExtractTo(
            [System.IO.Path]::Combine($TestDrive, 'myTestFolder'),
            $false)

        $file.FullName | Should -BeExactly ([System.IO.Path]::Combine($TestDrive, 'myTestFolder', $entry.Name))
    }

    It 'Can remove an entry in the source zip' {
        { ($zip | Get-ZipEntry).Remove() } |
            Should -Not -Throw

        $zip | Get-ZipEntry | Should -BeNullOrEmpty
    }
}
