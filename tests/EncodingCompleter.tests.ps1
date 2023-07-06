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
        (Complete 'Test-Completer utf').CompletionText
        CompletionMatches.
        CompletionText | Should -BeExactly $encodingSet.Where({ $_ -match '^utf' })
    }

    It 'Should not offer ansi as a completion result if the OS is not Windows' {
        if ($osIsWindows) {
            return
        }

            (TabExpansion2 -inputScript ($len = 'Test-Completer ansi') -cursorColumn $len.Length).
        CompletionMatches.
        CompletionText | Should -BeNullOrEmpty
    }
}
