$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([IO.Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [IO.Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([System.IO.Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'Compress-ZipArchive' -Tag 'Compress-ZipArchive' {
    BeforeAll {
        $sourceName = 'CompressZipArchiveTests'
        $destName = 'CompressZipArchiveExtract'
        $testpath = Join-Path $TestDrive $sourceName
        $extractpath = Join-Path $TestDrive $destName

        $structure = Get-Structure | ForEach-Object {
            $path = Join-Path $testpath $_
            if ($_.EndsWith('.txt')) {
                New-Item $path -ItemType File -Value (Get-Random) -Force
                return
            }
            New-Item $path -ItemType Directory -Force
        }

        $structure, $extractpath | Out-Null
    }

    It 'Can compress a folder and all its child items' {
        Compress-ZipArchive $testpath $extractpath -PassThru |
            Should -BeOfType ([System.IO.FileInfo])
    }

    It 'Should throw if the destination already exists' {
        { Compress-ZipArchive $testpath $extractpath } |
            Should -Throw
    }

    It 'Should overwrite if using -Force' {
        { Compress-ZipArchive $testpath $extractpath -Force } |
            Should -Not -Throw
    }

    It 'Extracted files should be exactly the same with the same structure' {
        BeforeAll {
            $map = @{}
            Get-ChildItem $testpath -Recurse | ForEach-Object {
                $relative = $_.FullName.Substring($testpath.Length)
                if ($_ -is [System.IO.FileInfo]) {
                    $map[$relative] = ($_ | Get-FileHash -Algorithm MD5).Hash
                    return
                }
                $map[$relative] = $null
            }

            Expand-Archive "$extractpath.zip" $extractpath

            $extractpath = Join-Path $extractpath $sourceName
            Get-ChildItem $extractpath -Recurse | ForEach-Object {
                $relative = $_.FullName.Substring($extractpath.Length)
                $map.ContainsKey($relative) | Should -BeTrue

                if ($_ -is [System.IO.FileInfo]) {
                    $thishash = ($_ | Get-FileHash -Algorithm MD5).Hash
                    $map[$relative] | Should -BeExactly $thishash
                }
            }
        }
    }

    It 'Can update entries if they exist' {
        $destination = [IO.Path]::Combine($TestDrive, 'UpdateTest', 'test.zip')
        $destinationExtract = [IO.Path]::Combine($TestDrive, 'UpdateTest')

        0..10 | ForEach-Object {
            New-Item (Join-Path $TestDrive ('file{0:D2}.txt' -f $_)) -ItemType File -Value 'hello'
        } | Compress-ZipArchive -Destination $destination

        Get-ChildItem $TestDrive -Filter *.txt | ForEach-Object {
            'world!' | Add-Content -LiteralPath $_.FullName
            $_
        } | Compress-ZipArchive -Destination $destination -Update

        Expand-Archive $destination $destinationExtract
        Get-ChildItem $destinationExtract -Filter *.txt | ForEach-Object {
            $_ | Get-Content | Should -BeExactly 'helloworld!'
        }
    }

    It 'Should skip the entry if the source and destination are the same' {
        Push-Location $TestDrive
        $zipname = 'testskipitself.zip'

        { Compress-ZipArchive $pwd.Path $zipname } |
            Should -Not -Throw

        { Compress-ZipArchive $pwd.Path $zipname -Force } |
            Should -Not -Throw

        { Compress-ZipArchive $pwd.Path $zipname -Update } |
            Should -Not -Throw

        Expand-Archive $zipname -DestinationPath skipitself

        Get-ChildItem skipitself -Recurse | ForEach-Object Name |
            Should -Not -Contain $zipname
    }
}
