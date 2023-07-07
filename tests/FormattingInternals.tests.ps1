$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'Module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'Formatting internals' {
    BeforeAll {
        $zip = New-Item (Join-Path $TestDrive test.zip) -ItemType File -Force
        'hello world!' | New-ZipEntry $zip.FullName -EntryPath helloworld.txt
        New-ZipEntry $zip.FullName -EntryPath afolder/
    }

    It 'Converts Length to their friendly representation' {
        [PSCompression.Internal._Format]::GetFormattedLength(1mb) |
            Should -BeExactly '1.00 MB'
    }

    It 'Gets the directory of an entry' {
        $zip | Get-ZipEntry | ForEach-Object {
            [PSCompression.Internal._Format]::GetDirectoryPath($_)
        } | Should -BeOfType ([string])
    }

    It 'Formats datetime instances' {
        [PSCompression.Internal._Format]::GetFormattedDate([datetime]::Now) |
            Should -BeExactly ([string]::Format([CultureInfo]::CurrentCulture,'{0,10:d} {0,8:t}', [datetime]::Now))
    }
}
