$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'EncodingTransformation Class' {
    BeforeAll {
        Add-Type -TypeDefinition '
        public static class Acp
        {
            [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
            public static extern int GetACP();
        }
        '

        $encodings = @{
            'ascii'            = [System.Text.ASCIIEncoding]::new()
            'bigendianunicode' = [System.Text.UnicodeEncoding]::new($true, $true)
            'bigendianutf32'   = [System.Text.UTF32Encoding]::new($true, $true)
            'oem'              = [Console]::OutputEncoding
            'unicode'          = [System.Text.UnicodeEncoding]::new()
            'utf8'             = [System.Text.UTF8Encoding]::new($false)
            'utf8bom'          = [System.Text.UTF8Encoding]::new($true)
            'utf8nobom'        = [System.Text.UTF8Encoding]::new($false)
            'utf32'            = [System.Text.UTF32Encoding]::new()
        }

        if ($osIsWindows) {
            $encodings['ansi'] = [System.Text.Encoding]::GetEncoding([Acp]::GetACP())
        }

        $transform = [PSCompression.EncodingTransformation]::new()
        $transform | Out-Null
    }

    It 'Transforms Encoding to Encoding' {
        $transform.Transform($ExecutionContext, [System.Text.Encoding]::UTF8) |
            Should -BeExactly ([System.Text.Encoding]::UTF8)
    }

    It 'Transforms a completion set to their Encoding Representations' {
        $encodings.GetEnumerator() | ForEach-Object {
            $transform.Transform($ExecutionContext, $_.Key) |
                Should -BeExactly $_.Value
            }
    }

    It 'Transforms CodePage to their Encoding Representations' {
        [System.Text.Encoding]::GetEncodings() | ForEach-Object {
            $transform.Transform($ExecutionContext, $_.CodePage) |
                Should -BeExactly $_.GetEncoding()
        }
    }

    It 'Throws if input value cannot be transformed' {
        { $transform.Transform($ExecutionContext, 'doesnotexist') } |
            Should -Throw
    }

    It 'Throws if the input value type is not Encoding, string or int' {
        { $transform.Transform($ExecutionContext, [type]) } |
            Should -Throw
    }
}
