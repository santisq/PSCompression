$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'ToFromStringCompression' {
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
}
