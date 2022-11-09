Install-Module PSCompression -Scope CurrentUser

$ProgressPreference = 'SilentlyContinue'

# Init vars
$folders   = 5
$files     = 20
$testPath  = Join-Path $pwd.ProviderPath -ChildPath testZip
$testPath  = New-Item $testPath -ItemType Directory -Force
$testCount = 5

'https://gist.githubusercontent.com/santisq/c051408701fc836415dcfc000c9bdfde/raw/c354f0a9b6f962a733d818bacdee64ae0c92ff85/New-DataSet.ps1',
'https://gist.githubusercontent.com/santisq/bd3d1d47c89f030be1b4e57b92baaddd/raw/0aecfd19e8db7119983c0a51dc2efaf9f5998424/measure-performance.ps1' |
    ForEach-Object { Invoke-RestMethod -Uri $_ | Invoke-Expression }

$createTestFiles = {
    param($TestPath, $Folders, $Files)

    foreach($folder in 1..$folders) {
        $outPath = Join-Path $testPath.FullName -ChildPath ("TestFolder{0:D2}" -f $folder)
        $null    = New-Item $outPath -Force -ItemType Directory
        foreach($file in 1..$files) {
            $fileName = Join-Path $outPath -ChildPath ("TestFile{0:D2}.csv" -f $file)
            New-DataSet -NumberOfObjects 10kb | Export-Csv $fileName
        }
    }
}

# Comment below line to avoid creating new files for a new test
& $createTestFiles $TestPath $Folders $Files

Measure-Performance @{
    'Compress-ZipArchive (Optimal)' = {
        $destination = Join-Path $pwd.ProviderPath -ChildPath "testZip-Compress-ZipArchive.zip"
        Compress-ZipArchive -Path $testPath -DestinationPath $destination -CompressionLevel Optimal
        Remove-Item $destination -Force
    }
    'Compress-Archive (Optimal)' = {
        $destination = Join-Path $pwd.ProviderPath -ChildPath "testZip-Compress-Archive.zip"
        Compress-Archive -Path $testPath -DestinationPath $destination -CompressionLevel Optimal
        Remove-Item $destination -Force
    }
} -TestCount $testCount

# comment below to avoid removing the test files
$testPath | Remove-Item -Recurse -Force