$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'Module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'Gzip Cmdlets' {
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
                Should -BeExactly ( -join @($content, $appendedContent | Get-Content))
        }

        It 'Can expand Gzip files with appended content to a destination file' {
            $expandGzipArchiveSplat = @{
                LiteralPath     = $destination
                DestinationPath = (Join-Path $TestDrive extractappended.txt)
                PassThru        = $true
            }

            Get-Content -LiteralPath (Expand-GzipArchive @expandGzipArchiveSplat).FullName |
                Should -BeExactly ( -join @($content, $appendedContent | Get-Content))
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
