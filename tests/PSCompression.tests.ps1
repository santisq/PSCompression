Describe PSCompression {
    BeforeAll {
        $ErrorActionPreference = 'Stop'

        $zip = New-Item (Join-Path $TestDrive test.zip) -ItemType File -Force

        $moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
        $manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

        if (-not (Get-Module -Name $moduleName -ErrorAction SilentlyContinue)) {
            Import-Module $manifestPath -ErrorAction Stop
        }

        $decoder = {
            param([byte[]] $bytes)

            try {
                $gzip = [System.IO.Compression.GZipStream]::new(
                    ($mem = [System.IO.MemoryStream]::new($bytes)),
                    [System.IO.Compression.CompressionMode]::Decompress)

                $out = [System.IO.MemoryStream]::new()
                $gzip.CopyTo($out)
            }
            finally {
                if($gzip -is [System.IDisposable]) {
                    $gzip.Dispose()
                }

                if($mem -is [System.IDisposable]) {
                    $mem.Dispose()
                }

                if($out -is [System.IDisposable]) {
                    $out.Dispose()
                    [System.Text.UTF8Encoding]::new().GetString($out.ToArray())
                }
            }
        }

        $zip, $decoder | Out-Null # Analyzer Rule is annoying :(
    }

    Context 'New-ZipEntry' -Tag 'New-ZipEntry' {
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

    Context 'Get-ZipEntry' -Tag 'Get-ZipEntry' {
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
    }

    Context 'Expand-ZipEntry' -Tag 'Expand-ZipEntry' {
        BeforeAll {
            $destination = New-Item (Join-Path $TestDrive -ChildPath 'ExtractTests') -ItemType Directory
            $zip | Get-ZipEntry | Remove-ZipEntry

            $structure = foreach($folder in 0..5) {
                $folder = 'testfolder{0:D2}/' -f $folder
                $folder
                foreach($file in 0..5) {
                    [System.IO.Path]::Combine($folder, 'testfile{0:D2}.txt' -f $file)
                }
            }

            $content = 'hello world!'
            $content | New-ZipEntry $zip.FullName -EntryPath $structure

            $destination, $structure, $content | Out-Null
        }

        It 'Can extract entries to a destination directory' {
            { $zip | Get-ZipEntry | Expand-ZipEntry -Destination $destination } |
                Should -Not -Throw
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

    Context 'ConvertTo & ConvertFrom GzipString' -Tag 'ConvertTo & ConvertFrom GzipString' {
        BeforeAll {
            $content = 'hello', 'world', '!'
            $content | Out-Null
        }

        It 'Can compress strings to gzip b64 strings from pipeline' {
            $encoded = { $content | ConvertTo-GzipString } |
                Should -Not -Throw -PassThru

            $encoded | ConvertFrom-GzipString |
                Should -BeExactly $contet
        }

        It 'Can compress strings to gzip b64 strings positionally' {
            $encoded = { ConvertTo-GzipString -InputObject $content } |
                Should -Not -Throw -PassThru

            $encoded | ConvertFrom-GzipString |
                Should -BeExactly $contet
        }

        It 'Can compress strings to gzip and output raw bytes' {
            [byte[]] $bytes = $content | ConvertTo-GzipString -AsByteStream
            $result = $decoder.InvokeReturnAsIs($bytes)
            $result.TrimEnd() | Should -BeExactly ($content -join [System.Environment]::NewLine)
        }

        It 'Can convert gzip b64 compressed string and output multi-line string' {
            $content | ConvertTo-GzipString |
                ConvertFrom-GzipString -Raw |
                ForEach-Object TrimEnd |
                Should -BeExactly ($content -join [System.Environment]::NewLine)
        }

        It 'Concatenates strings when the -NoNewLine switch is used' {
            $content | ConvertTo-GzipString -NoNewLine |
                ConvertFrom-GzipString |
                Should -BeExactly (-join $content)
        }
    }

    Context 'Compress & Expand GzipArchive' -Tag 'Compress & Expand GzipArchive' {
        BeforeAll {
            $content = 'hello world!' | New-Item (Join-Path $TestDrive content.txt)
            $appendedContent = 'this is appended content...' | New-Item (Join-Path $TestDrive appendedContent.txt)
            $destination = Join-Path $TestDrive -ChildPath test.gz

            $content, $appendedContent, $destination | Out-Null
        }

        It 'Can create a Gzip compressed file from a specified path' {
            $content |
                Compress-GzipArchive -DestinationPath $destination -PassThru |
                Expand-GzipArchive |
                Should -BeExactly ($content | Get-Content)
        }

        It 'Can append content to a Gzip compressed file from a specified path' {
            $appendedContent |
                Compress-GzipArchive -DestinationPath $destination -PassThru -Update |
                Expand-GzipArchive |
                Should -BeExactly (-join @($content, $appendedContent | Get-Content))
        }

        It 'Should not overwrite an existing Gzip file without -Force' {
            { $content | Compress-GzipArchive -DestinationPath $destination } |
                Should -Throw
        }

        It 'Can overwrite an existing Gzip file with -Force' {
            { $content | Compress-GzipArchive -DestinationPath $destination -Force } |
                Should -Not -Throw

            Expand-GzipArchive -LiteralPath $destination |
                Should -BeExactly ($content | Get-Content)
        }

        It 'Can expand Gzip files to a destination file' {
            $expandGzipArchiveSplat = @{
                LiteralPath     = $destination
                DestinationPath = (Join-Path $TestDrive extract.txt)
                PassThru        = $true
            }

            Get-Content -LiteralPath (Expand-GzipArchive @expandGzipArchiveSplat).FullName |
                Should -BeExactly ($content | Get-Content)
        }
    }
}
