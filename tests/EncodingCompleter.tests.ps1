$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'Module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

BeforeAll {
    $encodingSet = @(
        'ascii'
        'bigendianUtf32'
        'unicode'
        'utf8'
        'utf8NoBOM'
        'bigendianUnicode'
        'oem'
        'utf8BOM'
        'utf32'

        if ($osIsWindows) {
            'ansi'
        }
    )

    $encodingSet | Out-Null
}

Describe 'EncodingCompleter Class' {
    It 'Completes results from a completion set' {
        (Complete 'Test-Completer ').CompletionText |
            Should -BeExactly $encodingSet
    }

    It 'Completes results from a word to complete' {
        (Complete 'Test-Completer utf').CompletionText |
            Should -BeExactly ($encodingSet -match '^utf')
    }

    It 'Should not offer ansi as a completion result if the OS is not Windows' {
        if ($osIsWindows) {
            return
        }

        (Complete 'Test-Completer ansi').CompletionText |
            Should -BeNullOrEmpty
    }
}
