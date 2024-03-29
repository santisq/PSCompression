﻿$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'Gzip Cmdlets' {
    Context 'ConvertFrom-GzipString' -Tag 'ConvertFrom-GzipString' {
        It 'Should throw on a non b64 encoded input' {
            { 'foo' | ConvertFrom-GzipString } |
                Should -Throw
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
            $result = Decode $bytes
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
            $testString = 'hello world!'
            $content = $testString | New-Item (Join-Path $TestDrive content.txt)
            $appendedContent = 'this is appended content...' | New-Item (Join-Path $TestDrive appendedContent.txt)
            $destination = Join-Path $TestDrive -ChildPath test.gz
            $testString, $content, $appendedContent, $destination | Out-Null
        }

        It 'Can create a Gzip compressed file from a specified path' {
            $content |
                Compress-GzipArchive -DestinationPath $destination -PassThru |
                Expand-GzipArchive |
                Should -BeExactly ($content | Get-Content)
        }

        It 'Outputs a single multiline string when using the -Raw switch' {
            Expand-GzipArchive $destination -Raw |
                Should -BeExactly ($content | Get-Content -Raw)
        }

        It 'Can append content to a Gzip compressed file from a specified path' {
            $appendedContent |
                Compress-GzipArchive -DestinationPath $destination -PassThru -Update |
                Expand-GzipArchive |
                Should -BeExactly (-join @($content, $appendedContent | Get-Content))
        }

        It 'Can expand Gzip files with appended content to a destination file' {
            $expandGzipArchiveSplat = @{
                LiteralPath     = $destination
                DestinationPath = (Join-Path $TestDrive extractappended.txt)
                PassThru        = $true
            }

            Get-Content -LiteralPath (Expand-GzipArchive @expandGzipArchiveSplat).FullName |
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

        It 'Should throw if expanding to an existing file' {
            $expandGzipArchiveSplat = @{
                LiteralPath     = $destination
                DestinationPath = (Join-Path $TestDrive extract.txt)
            }

            { Expand-GzipArchive @expandGzipArchiveSplat } |
                Should -Throw
        }

        It 'Can append content with -Update' {
            $expandGzipArchiveSplat = @{
                LiteralPath     = $destination
                DestinationPath = (Join-Path $TestDrive extract.txt)
                PassThru        = $true
                Update          = $true
            }

            $appendedContent = $testString + $testString
            Get-Content -LiteralPath (Expand-GzipArchive @expandGzipArchiveSplat).FullName |
                Should -BeExactly $appendedContent
        }

        It 'Can overwrite the destination file with -Force' {
            $expandGzipArchiveSplat = @{
                LiteralPath     = $destination
                DestinationPath = (Join-Path $TestDrive extract.txt)
                PassThru        = $true
                Force           = $true
            }

            Get-Content -LiteralPath (Expand-GzipArchive @expandGzipArchiveSplat).FullName |
                Should -BeExactly $testString
        }

        It 'Should throw if the Path is not an Archive' {
            { Expand-GzipArchive $pwd } | Should -Throw
        }

        It 'Should throw if the Path is not a Gzip Archive' {
            { Expand-GzipArchive -LiteralPath (Join-Path $TestDrive content.txt) } |
                Should -Throw
        }

        It 'Can create folders when the destination directory does not exist' {
            $expandGzipArchiveSplat = @{
                LiteralPath     = $destination
                DestinationPath = [IO.Path]::Combine($TestDrive, 'does', 'not', 'exist', 'extract.txt')
                PassThru        = $true
                Force           = $true
            }

            { Expand-GzipArchive @expandGzipArchiveSplat } |
                Should -Not -Throw
        }

        It 'Should throw if trying to compress a directory' {
            $folders = 0..5 | ForEach-Object {
                New-Item (Join-Path $TestDrive "folder $_") -ItemType Directory
            }

            { Compress-GzipArchive $folders.FullName -Destination $destination -Force } |
                Should -Throw
        }

        Context 'Compress-GzipArchive with InputBytes' {
            BeforeAll {
                $path = [IO.Path]::Combine($TestDrive, 'this', 'is', 'atest', 'file')
                $testStrings = 'hello', 'world', '!'
                $path, $testStrings | Out-Null
            }

            It 'Can create compressed files from input bytes' {
                $testStrings | ConvertTo-GzipString -AsByteStream |
                    Compress-GzipArchive -DestinationPath $path -PassThru |
                    Expand-GzipArchive |
                    Should -BeExactly $testStrings
            }

            It 'Can append to compressed files from input bytes' {
                $testStrings | ConvertTo-GzipString -AsByteStream |
                    Compress-GzipArchive -DestinationPath $path -PassThru -Update |
                    Expand-GzipArchive |
                    Should -BeExactly ($testStrings + $testStrings)
            }

            It 'Can overwrite a compressed file from input bytes' {
                $testStrings | ConvertTo-GzipString -AsByteStream |
                    Compress-GzipArchive -DestinationPath $path -PassThru -Force |
                    Expand-GzipArchive |
                    Should -BeExactly $testStrings
            }
        }
    }
}
