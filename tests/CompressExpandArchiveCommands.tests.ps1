using namespace System.IO
using namespace System.Collections.Generic

$ErrorActionPreference = 'Stop'

$moduleName = (Get-Item ([Path]::Combine($PSScriptRoot, '..', 'module', '*.psd1'))).BaseName
$manifestPath = [Path]::Combine($PSScriptRoot, '..', 'output', $moduleName)

Import-Module $manifestPath
Import-Module ([Path]::Combine($PSScriptRoot, 'shared.psm1'))

Describe 'Compress & Expand Archive Commands' -Tag 'Compress & Expand Archive Commands' {
    BeforeAll {
        $algos = [PSCompression.Algorithm].GetEnumValues()

        $sourceName = 'CompressArchiveTests'
        $destName = 'CompressArchiveExtract'

        $testpath = Join-Path $TestDrive $sourceName
        $extractpath = Join-Path $TestDrive $destName

        $fileCount = $dirCount = 0
        $structure = Get-Structure | ForEach-Object {
            $newItemSplat = @{
                ItemType = ('Directory', 'File')[$_.EndsWith('.txt')]
                Value    = Get-Random
                Force    = $true
                Path     = Join-Path $testpath $_
            }

            New-Item @newItemSplat

            if ($newItemSplat['ItemType'] -eq 'Directory') {
                $dirCount++
            }
            else {
                $fileCount++
            }
        }

        $dirCount++ # Includes the folder itself
        $extractpath, $structure, $algos | Out-Null
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
        Get-ChildItem $TestDrive -File | ForEach-Object {
            $destination = [Path]::Combine($TestDrive, "ExpandTest_$($_.Extension.TrimStart('.'))")

            if ($_.Extension -eq '.zip') {
                $_ | Expand-Archive -DestinationPath $destination
            }
            elseif ($_.Extension -match '\.tar.*') {
                $_ | Expand-TarArchive -Destination $destination
            }
            else {
                return
            }

            $expanded = Get-ChildItem $destination -Recurse
            $files, $dirs = $expanded.Where({ $_ -is [FileInfo] }, 'Split')

            $files | Should -HaveCount $fileCount
            $dirs | Should -HaveCount $dirCount
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
        $name = 'testskipitself'

        { Compress-ZipArchive $pwd.Path $name } |
            Should -Not -Throw

        { Compress-ZipArchive $pwd.Path $name -Force } |
            Should -Not -Throw

        { Compress-ZipArchive $pwd.Path $name -Update } |
            Should -Not -Throw

        $destination = [guid]::NewGuid()
        Expand-Archive "${name}.zip" -DestinationPath $destination

        Get-ChildItem $destination -Recurse | ForEach-Object Name |
            Should -Not -Contain "${name}.zip"

        $archive = [Queue[FileInfo]]::new()
        $algos | ForEach-Object {
            $currentName = "${name}_${_}"
            { Compress-TarArchive $pwd.Path $currentName -Algorithm $_ } |
                Should -Not -Throw


            {
                $result = Compress-TarArchive $pwd.Path $currentName -Algorithm $_ -Force -PassThru
                $archive.Enqueue($result)
            } | Should -Not -Throw

            $info = $archive.Dequeue()
            $info | Should -BeOfType ([FileInfo])
            Expand-TarArchive $info.FullName -Destination $currentName
            Get-ChildItem $currentName -Recurse | ForEach-Object Name |
                Should -Not -Contain $info.Name
        }
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
