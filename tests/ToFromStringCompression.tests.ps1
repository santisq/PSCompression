using namespace System.IO
using namespace System.IO.Compression

$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'To & From String Compression' {
    BeforeAll {
        $conversionCommands = @{
            'ConvertFrom-BrotliString'  = 'ConvertTo-BrotliString'
            'ConvertFrom-DeflateString' = 'ConvertTo-DeflateString'
            'ConvertFrom-GzipString'    = 'ConvertTo-GzipString'
            'ConvertFrom-ZlibString'    = 'ConvertTo-ZlibString'
        }

        $conversionCommands | Out-Null
    }

    Context 'ConvertFrom Commands' -Tag 'ConvertFrom Commands' {
        It 'Should throw on a non b64 encoded input' {
            $conversionCommands.Keys | ForEach-Object {
                { 'foo' | & $_ } | Should -Throw -ExceptionType ([FormatException])
            }
        }
    }

    Context 'ConvertTo & ConvertFrom General Usage' -Tag 'ConvertTo & ConvertFrom General Usage' {
        BeforeAll {
            $content = 'hello', 'world', '!'
            $content | Out-Null
        }

        It 'Can compress strings and expand strings' {
            foreach ($level in [CompressionLevel].GetEnumNames()) {
                $conversionCommands.Keys | ForEach-Object {
                    $encoded = $content | & $conversionCommands[$_] -CompressionLevel $level
                    $encoded | & $_ | Should -BeExactly $content
                }
            }
        }

        It 'Can compress strings outputting raw bytes' {
            $conversionCommands.Keys | ForEach-Object {
                [byte[]] $bytes = $content | & $conversionCommands[$_] -AsByteStream
                $result = [Convert]::ToBase64String($bytes) | & $_
                $result | Should -BeExactly $content
            }
        }

        It 'Can expand b64 compressed strings and output a multi-line string' {
            $contentAsString = $content -join [Environment]::NewLine
            $conversionCommands.Keys | ForEach-Object {
                $content | & $conversionCommands[$_] | & $_ -Raw |
                    ForEach-Object TrimEnd | Should -BeExactly $contentAsString
            }
        }

        It 'Concatenates strings when the -NoNewLine switch is used' {
            $contentAsString = -join $content
            $conversionCommands.Keys | ForEach-Object {
                $content | & $conversionCommands[$_] -NoNewLine | & $_ |
                    Should -BeExactly $contentAsString
            }
        }
    }
}
