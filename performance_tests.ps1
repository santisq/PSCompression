$ProgressPreference = 'SilentlyContinue'

# Init vars
$folders   = 5
$files     = 20
$fileSize  = 300kb
$testPath  = Join-Path $pwd.ProviderPath -ChildPath testZip
$testPath  = New-Item $testPath -ItemType Directory -Force
$testCount = 5

$createTestFiles = {
    param($TestPath, $Folders, $Files, $FileSize)
    # Charmap to create random junk files
    $ran     = [random]::new()
    $charmap = ''
    [char[]]([char]'A'..[char]'Z') +
    [char[]]([char]'a'..[char]'z') +
    0..10 | ForEach-Object { $charmap += $_.ToString() }

    # Create random files and folders in `$testPath`
    foreach($folder in 1..$folders) {
        $outPath = Join-Path $testPath.FullName -ChildPath ("TestFolder{0:D2}" -f $folder)
        $null    = New-Item $outPath -Force -ItemType Directory
        foreach($file in 1..$files) {
            $fileName = Join-Path $outPath -ChildPath ("TestFile{0:D2}.txt" -f $file)
            $content  = [char[]]::new($fileSize)

            for($z = 0; $z -lt $content.Length; $z++) {
                $content[$z] = $charmap[$ran.Next($charmap.Length)]
            }
            $memStream = [IO.MemoryStream]::new([byte[]] $content)
            $thisFile  = [IO.File]::Create($fileName, $fileSize)
            $memStream.CopyTo($thisFile)
            $thisFile, $memStream | ForEach-Object Dispose
        }
    }
}

# Comment below line to avoid creating new files for a new test
& $createTestFiles $TestPath $Folders $Files $FileSize

# bring the function to this session
Invoke-RestMethod 'https://raw.githubusercontent.com/santysq/Compress-File/main/Compress-File.ps1' |
    Invoke-Expression

$tests = @{
    'Compress-File (Optimal)' = {
        $destination = Join-Path $pwd.ProviderPath -ChildPath "testZip-Compress-File.zip"
        Compress-File -Path $testPath -DestinationPath $destination -CompressionLevel Optimal
        Remove-Item $destination -Force
    }
    'Compress-Archive (Optimal)' = {
        $destination = Join-Path $pwd.ProviderPath -ChildPath "testZip-Compress-Archive.zip"
        Compress-Archive -Path $testPath -DestinationPath $destination -CompressionLevel Optimal
        Remove-Item $destination -Force
    }
}

$allTests = 1..$testCount | ForEach-Object {
    foreach($test in $tests.GetEnumerator()) {
        [pscustomobject]@{
            TestRun           = $_
            Test              = $test.Key
            TotalMilliseconds = [math]::Round((Measure-Command { & $test.Value }).TotalMilliseconds, 2)
        }
    }
} | Sort-Object TotalMilliseconds

$average = $allTests | Group-Object Test | ForEach-Object {
    [pscustomobject]@{
        Test          = $_.Name
        Average       = [Linq.Enumerable]::Average([double[]] $_.Group.TotalMilliseconds)
        RelativeSpeed = 0
    }
} | Sort-Object Average

for($i = 0; $i -lt $average.Count; $i++) {
    if($i) {
        $average[$i].RelativeSpeed = ($average[$i].Average / $average[0].Average).ToString('N2') + 'x'
        continue
    }
    $average[$i].RelativeSpeed = '1x'
}

$allTests | Format-Table -AutoSize
$average  | Format-Table -AutoSize

# comment below to avoid removing the test files
$testPath | Remove-Item -Recurse -Force