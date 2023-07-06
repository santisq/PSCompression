function Complete {
    [OutputType([System.Management.Automation.CompletionResult])]
    param([string] $Expression)

    end {
        [System.Management.Automation.CommandCompletion]::CompleteInput(
            $Expression,
            $Expression.Length,
            $null).CompletionMatches
    }
}

function Decode {
    param([byte[]] $bytes)

    end {
        try {
            $gzip = [System.IO.Compression.GZipStream]::new(
                ($mem = [System.IO.MemoryStream]::new($bytes)),
                [System.IO.Compression.CompressionMode]::Decompress)

            $out = [System.IO.MemoryStream]::new()
            $gzip.CopyTo($out)
        }
        finally {
            if ($gzip -is [System.IDisposable]) {
                $gzip.Dispose()
            }

            if ($mem -is [System.IDisposable]) {
                $mem.Dispose()
            }

            if ($out -is [System.IDisposable]) {
                $out.Dispose()
                [System.Text.UTF8Encoding]::new().GetString($out.ToArray())
            }
        }
    }
}

function Test-Completer {
    param(
        [ArgumentCompleter([PSCompression.EncodingCompleter])]
        [string] $Test
    )
}

$osIsWindows = [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform(
    [System.Runtime.InteropServices.OSPlatform]::Windows)

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'Module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)
$osIsWindows, $manifestPath | Out-Null

$exportModuleMemberSplat = @{
    Variable = 'moduleName', 'manifestPath', 'osIsWindows'
    Function = 'Decode', 'Complete', 'Test-Completer'
}

Export-ModuleMember @exportModuleMemberSplat
