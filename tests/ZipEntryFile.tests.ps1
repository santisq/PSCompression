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
        ($zip | Get-ZipEntry).Type | Should -BeExactly ([PSCompression.ZipEntryType]::Archive)
    }

    It 'Should Open the source zip' {
        try {
            $stream = ($zip | Get-ZipEntry).OpenRead()
            $stream | Should -BeOfType ([System.IO.Compression.ZipArchive])
        }
        finally {
            if ($stream -is [System.IDisposable]) {
                $stream.Dispose()
            }
        }
    }
}
