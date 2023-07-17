$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'ZipEntry Cmdlets' {
    BeforeAll {
        $zip = New-Item (Join-Path $TestDrive test.zip) -ItemType File -Force
        $file = New-Item ([System.IO.Path]::Combine($TestDrive, 'someFile.txt')) -ItemType File -Value 'foo'
        $zip, $file | Out-Null
    }

    Context 'New-ZipEntry' -Tag 'New-ZipEntry' {
        It 'Should throw if -Destination is not a zip file' {
            { New-ZipEntry -Destination $file.FullName -EntryPath foo } |
                Should -Throw
        }

        It 'Should throw if -Destination is a Directory' {
            { New-ZipEntry -Destination $pwd.Path -EntryPath bar } |
                Should -Throw
        }

        It 'Should throw if -Destination is a provider path' {
            { New-ZipEntry -Destination function: -EntryPath bar } |
                Should -Throw
        }

        It 'Should throw if -Source is a Directory' {
            { New-ZipEntry -Destination $zip.FullName -EntryPath baz -SourcePath $pwd.FullName } |
                Should -Throw
        }

        It 'Should throw if -Source is not a valid file path' {
            { New-ZipEntry -Destination $zip.FullName -EntryPath foo -SourcePath doesnotexist } |
                Should -Throw
        }

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
            { New-ZipEntry $zip.FullName -EntryPath foo.txt, bar.txt, baz.txt } |
                Should -Throw
        }

        It 'Can replace an existing entry with -Force' {
            { New-ZipEntry $zip.FullName -EntryPath foo.txt, bar.txt, baz.txt -Force } |
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

    Context 'Get-ZipEntry' -Tag 'Get-ZipEntry' {
        It 'Can list entries in a zip archive' {
            @($zip | Get-ZipEntry).Count -gt 0 |
                Should -BeGreaterThan 0
        }

        It 'Should throw when not targetting a FileSystem Provider Path' {
            { Get-ZipEntry function:\* } | Should -Throw
        }

        It 'Should throw when the path is not a Zip' {
            { $file | Get-ZipEntry } | Should -Throw
        }

        It 'Should throw if the path is not a file' {
            { $pwd | Get-ZipEntry } | Should -Throw
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

    Context 'Get-ZipEntryContent' -Tag 'Get-ZipEntryContent' {
        It 'Can read content from zip file entries' {
            $zip | Get-ZipEntry -EntryType Archive |
                Get-ZipEntryContent |
                Should -BeOfType ([string])
        }

        It 'Can read bytes from zip file entries' {
            $zip | Get-ZipEntry -EntryType Archive |
                Get-ZipEntryContent -AsByteStream |
                Should -BeOfType ([byte])
        }

        It 'Can output a byte array when using the -Raw switch' {
            $zip | Get-ZipEntry -EntryType Archive |
                Get-ZipEntryContent -AsByteStream -Raw |
                Should -BeOfType ([byte[]])
        }

        It 'Should not attempt to read a directory entry' {
            { $zip | Get-ZipEntry -EntryType Directory | Get-ZipEntry } |
                Should -Throw
        }
    }

    Context 'Remove-ZipEntry' -Tag 'Remove-ZipEntry' {
        It 'Can remove file entries' {
            { $zip | Get-ZipEntry -EntryType Archive | Remove-ZipEntry } |
                Should -Not -Throw

            $zip | Get-ZipEntry | Should -Not -BeOfType ([PSCompression.ZipEntryFile])
        }

        It 'Can remove directory entries' {
            $entries = $zip | Get-ZipEntry -EntryType Directory
            { Remove-ZipEntry -InputObject $entries } | Should -Not -Throw
            $zip | Get-ZipEntry | Should -Not -BeOfType ([PSCompression.ZipEntryDirectory])
        }

        It 'Should not throw if there are no entries to remove' {
            { $zip | Get-ZipEntry | Remove-ZipEntry } |
                Should -Not -Throw
        }

        It 'Should be empty after all entries have been removed' {
            $zip | Get-ZipEntry | Should -BeNullOrEmpty
        }

    }

    Context 'Set-ZipEntryContent' -Tag 'Set-ZipEntryContent' {
        BeforeAll {
            $content = 'hello', 'world', '!'
            [byte[]] $bytes = $content | ForEach-Object {
                [System.Text.Encoding]::UTF8.GetBytes($_)
            }
            $entry = New-ZipEntry $zip.FullName -EntryPath test\helloworld.txt
            $entry, $content, $bytes | Out-Null
        }

        It 'Can write new content to a zip file entry' {
            $content | Set-ZipEntryContent $entry
            $entry | Get-ZipEntryContent | Should -BeExactly $content
            $entry | Get-ZipEntryContent -Raw |
                ForEach-Object TrimEnd |
                Should -BeExactly ($content -join [System.Environment]::NewLine)
        }

        It 'Can append content to a zip file entry' {
            $newContent = $content + $content
            $content | Set-ZipEntryContent $entry -Append
            $entry | Get-ZipEntryContent | Should -BeExactly $newContent
            $entry | Get-ZipEntryContent -Raw |
                ForEach-Object TrimEnd |
                Should -BeExactly ($newContent -join [System.Environment]::NewLine)
        }

        It 'Can write raw bytes to a zip file entry' {
            $bytes | Set-ZipEntryContent $entry -AsByteStream
            $entry | Get-ZipEntryContent -AsByteStream |
                Should -BeExactly $bytes
        }

        It 'Can append raw bytes to a zip file entry' {
            $newByteArray = [byte[]]::new($bytes.Length * 2)
            $bytes.CopyTo($newByteArray, 0)
            $bytes.CopyTo($newByteArray, $bytes.Length)

            $bytes | Set-ZipEntryContent $entry -AsByteStream -Append
            $entry | Get-ZipEntryContent -AsByteStream |
                Should -BeExactly $newByteArray
        }

        It 'Outputs the source entry with -PassThru' {
            'hello world!' | Set-ZipEntryContent $entry -PassThru |
                Should -BeOfType ([PSCompression.ZipEntryFile])
        }
    }

    Context 'Expand-ZipEntry' -Tag 'Expand-ZipEntry' {
        BeforeAll {
            $destination = New-Item (Join-Path $TestDrive -ChildPath 'ExtractTests') -ItemType Directory
            $zip | Get-ZipEntry | Remove-ZipEntry
            $structure = Get-Structure
            $content = 'hello world!'
            $content | New-ZipEntry $zip.FullName -EntryPath $structure
            $destination, $structure, $content | Out-Null
        }

        It 'Can extract entries to a destination directory' {
            { $zip | Get-ZipEntry | Expand-ZipEntry -Destination $destination } |
                Should -Not -Throw
        }

        It 'Should throw when -Destination is an invalid path' {
            { $zip | Get-ZipEntry | Expand-ZipEntry -Destination function: } |
                Should -Throw
        }

        It 'Should throw if the destination path argument belongs to a file' {
            { $zip | Get-ZipEntry | Expand-ZipEntry -Destination $zip.FullName } |
                Should -Throw
        }

        It 'Should not overwrite files without -Force' {
            { $zip | Get-ZipEntry | Expand-ZipEntry -Destination $destination } |
                Should -Throw
        }

        It 'Can overwrite files if using -Force' {
            { $zip | Get-ZipEntry | Expand-ZipEntry -Destination $destination -Force } |
                Should -Not -Throw
        }

        It 'Should preserve the file content of the extracted entries' {
            $expanded = $zip | Get-ZipEntry |
                Expand-ZipEntry -Destination $destination -Force -PassThru

            $expanded | Should -HaveCount $structure.Count

            Get-ChildItem -LiteralPath $destination -Recurse -File |
                ForEach-Object { $_ | Get-Content | Should -BeExactly $content }
        }
    }
}
