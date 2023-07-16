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

function Get-Structure {
    foreach ($folder in 0..5) {
        $folder = 'testfolder{0:D2}/' -f $folder
        $folder
        foreach ($file in 0..5) {
            [System.IO.Path]::Combine($folder, 'testfile{0:D2}.txt' -f $file)
        }
    }
}

$osIsWindows = [System.Runtime.InteropServices.RuntimeInformation]::IsOSPlatform(
    [System.Runtime.InteropServices.OSPlatform]::Windows)

$osIsWindows | Out-Null

$exportModuleMemberSplat = @{
    Variable = 'moduleName', 'manifestPath', 'osIsWindows'
    Function = 'Decode', 'Complete', 'Test-Completer', 'Get-Structure'
}

Export-ModuleMember @exportModuleMemberSplat
