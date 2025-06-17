using namespace System.IO

$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'CompressArchive Commands' -Tag 'CompressArchive Commands' {
    BeforeAll {
        $algos = [PSCompression.Algorithm].GetEnumValues()
        $sourceName = 'CompressArchiveTests'
        $destName = 'CompressArchiveExtract'
        $testpath = Join-Path $TestDrive $sourceName
        $extractpath = Join-Path $TestDrive $destName
        $structure = Get-Structure | ForEach-Object {
            $newItemSplat = @{
                ItemType = ('Directory', 'File')[$_.EndsWith('.txt')]
                Value    = (Get-Random)
                Force    = $true
                Path     = Join-Path $testpath $_
            }

            New-Item @newItemSplat
        }

        $structure, $extractpath, $algos | Out-Null
    }

    It 'Can compress a folder and all its child items' {
        Compress-ZipArchive $testpath $extractpath -PassThru |
            Should -BeOfType ([FileInfo])

        $algos | ForEach-Object {
            Compress-TarArchive $testpath $extractpath -PassThru -Algorithm $_ |
                Should -BeOfType ([FileInfo])
        }
    }

    It 'Should throw if the destination already exists' {
        { Compress-ZipArchive $testpath $extractpath } |
            Should -Throw -ExceptionType ([IOException])

        $algos | ForEach-Object {
            { Compress-TarArchive $testpath $extractpath -Algorithm $_ } |
                Should -Throw -ExceptionType ([IOException])
        }
    }

    It 'Should overwrite if using -Force parameter' {
        { Compress-ZipArchive $testpath $extractpath -Force } |
            Should -Not -Throw

        $algos | ForEach-Object {
            { Compress-TarArchive $testpath $extractpath -Algorithm $_ -Force } |
                Should -Not -Throw
        }
    }

    It 'Extracted files should be exactly the same with the same structure' {
        BeforeAll {
            $map = @{}
            Get-ChildItem $testpath -Recurse | ForEach-Object {
                $relative = $_.FullName.Substring($testpath.Length)
                if ($_ -is [FileInfo]) {
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

                if ($_ -is [FileInfo]) {
                    $thishash = ($_ | Get-FileHash -Algorithm MD5).Hash
                    $map[$relative] | Should -BeExactly $thishash
                }
            }
        }
    }

    It 'Can update entries if they exist' {
        $destination = [Path]::Combine($TestDrive, 'UpdateTest', 'test.zip')
        $destinationExtract = [Path]::Combine($TestDrive, 'UpdateTest')

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

    It 'Should skip items that match the exclusion patterns' {
        Remove-Item "$extractpath.zip" -Force
        Compress-ZipArchive $testpath $extractpath -Exclude *testfile00*, *testfolder05*
        Expand-Archive "$extractpath.zip" $extractpath
        Get-ChildItem $extractpath -Recurse | ForEach-Object {
            $_.FullName | Should -Not -BeLike *testfile00*
            $_.FullName | Should -Not -BeLike *testfolder05*
        }
    }
}
