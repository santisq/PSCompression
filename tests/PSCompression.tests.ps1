BeforeAll {
    $ErrorActionPreference = 'Stop'

    $zip = New-Item (Join-Path $TestDrive test.zip) -ItemType File -Force
    $zip | Out-Null # Analyzer Rule is annoying :(

    $moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
    $manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

    if (-not (Get-Module -Name $moduleName -ErrorAction SilentlyContinue)) {
        Import-Module $manifestPath -ErrorAction Stop
    }
}

Describe 'New-ZipEntry' {
    It 'Can create new zip file entries' {
        New-ZipEntry $zip.FullName -EntryPath test\newentry.txt |
            Should -BeOfType ([PSCompression.ZipEntryFile])
    }
    It 'Can create new zip directory entries' {
        New-ZipEntry $zip.FullName -EntryPath test\ |
            Should -BeOfType ([PSCompression.ZipEntryDirectory])
    }
    It 'Can create multiple entries' {
        New-ZipEntry $zip.FullName -EntryPath foo.txt, bar.txt, baz.txt |
            Should -HaveCount 3
    }
    It 'Should not create an entry with the same path' {
        { New-ZipEntry $zip.FullName -EntryPath test\newentry.txt } |
            Should -Throw
    }
    It 'Can replace an existing entry with -Force' {
        { New-ZipEntry $zip.FullName -EntryPath test\newentry.txt -Force } |
            Should -Not -Throw
    }
    It 'Can create entries with content from value' {
        'hello world!' | New-ZipEntry $zip.FullName -EntryPath helloworld.txt |
            Get-ZipEntryContent |
            Should -Be 'hello world!'
    }
    It 'Can create entries with content from file' {
        $newItemSplat = @{
            ItemType = 'File'
            Force    = $true
            Path     = (Join-Path $TestDrive helloworld.txt)
        }

        $item = 'hello world!' | New-Item @newItemSplat

        $newZipEntrySplat = @{
            EntryPath   = 'helloworldfromafile.txt'
            SourcePath  = $item.FullName
            Destination = $zip.FullName
        }

        New-ZipEntry @newZipEntrySplat |
            Get-ZipEntryContent |
            Should -Be 'hello world!'
    }
}

Describe 'Get-ZipEntry' {
    It 'Can list entries in a zip archive' {
        @($zip | Get-ZipEntry).Count -gt 0 |
            Should -BeGreaterThan 0
    }
    It 'Can list zip file entries' {
        $zip | Get-ZipEntry -EntryType Archive |
            Should -BeOfType ([PSCompression.ZipEntryFile])
    }
    It 'Can list zip directory entries' {
        $zip | Get-ZipEntry -EntryType Directory |
            Should -BeOfType ([PSCompression.ZipEntryDirectory])
    }
    It 'Can list a specific entry with the -Include parameter' {
        $zip | Get-ZipEntry -Include test/newentry.txt |
            Should -Not -BeNullOrEmpty
    }
    It 'Can exclude entries using the -Exclude parameter' {
        $zip | Get-ZipEntry -Exclude *.txt |
            ForEach-Object { [System.IO.Path]::GetExtension($_.EntryRelativePath) } |
            Should -Not -Be '.txt'
    }
}

Describe 'Get-ZipEntryContent' {
    It 'Can read content from zip file entries' {

    }
}
