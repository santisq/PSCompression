## GZIP EXAMPLES

### Example 1: Strings to Gzip compressed Base 64 encoded string

```powershell
PS ..\pwsh> $strings = 'hello', 'world', '!'

# With positional binding
PS ..\pwsh> ConvertTo-GzipString $strings

H4sIAAAAAAAEAMtIzcnJ5+Uqzy/KSeHlUuTlAgBLr/K2EQAAAA==

# Or pipeline input, both work
PS ..\pwsh> $strings | ConvertTo-GzipString

H4sIAAAAAAAEAMtIzcnJ5+Uqzy/KSeHlUuTlAgBLr/K2EQAAAA==
```

### Example 2: Expanding compressed strings

```powershell
PS ..\pwsh> ConvertFrom-GzipString H4sIAAAAAAAACstIzcnJ5+Uqzy/KSeHlUuTlAgBLr/K2EQAAAA==

hello
world
!
```

### Example 3: Demonstrates how `-NoNewLine` works

New lines are preserved when the function receives an array of strings.

```powershell
PS ..\pwsh> $strings | ConvertTo-GzipString | ConvertFrom-GzipString

hello
world
!

# When using the switch, all strings are concatenated
PS ..\pwsh> $strings | ConvertTo-GzipString -NoNewLine | ConvertFrom-GzipString

helloworld!
```

### Example 4: Create a Gzip compressed file from a string

Demonstrates how `-Raw` works on `ConvertTo-GzipString`. Sends the compressed bytes to `Compress-GzipArchive`.

```powershell
PS ..\pwsh> 'hello world!' | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip
```

### Example 5: Append content to the previous Gzip file

Demonstrates how `-Update` works.

```powershell
PS ..\pwsh> 'this is new content...' | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip -Update
```

### Example 6: Expanding a Gzip file

Output goes to the Success Stream when `-DestinationPath` is not bound.

```powershell
PS ..\pwsh> Expand-GzipArchive .\files\file.gzip

hello world!
this is new content...
```

### Example 7: Replace the previous Gzip file with new content

Demonstrates how `-Force` works.

```powershell
PS ..\pwsh> $lorem = Invoke-RestMethod loripsum.net/api/10/long/plaintext
PS ..\pwsh> $lorem | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip -Force
```



### Example 8: Expanding a Gzip file outputting to a file

```powershell
PS ..\pwsh> Expand-GzipArchive .\files\file.gzip -DestinationPath .\files\file.txt
```

Checking Length Difference

```powershell
PS ..\pwsh> Get-Item -Path .\files\file.gzip, .\files\file.txt |
    Select-Object Name, Length

Name      Length
----      ------
file.gzip   3168
file.txt    6857
```

Checking Integrity between expanded and original

```powershell
PS ..\pwsh> $lorem | Set-Content .\files\strings.txt
PS ..\pwsh> Get-FileHash -Path .\files\file.txt, .\files\strings.txt -Algorithm MD5

Hash
----
E22E4786F9666E6E7F4A512B3714C517
E22E4786F9666E6E7F4A512B3714C517
```

### Example 9: Compressing multiple files into one Gzip file

Due to the nature of Gzip without Tar, all file contents are merged into one file.

```powershell
PS ..\pwsh> 0..10 | ForEach-Object {
    Invoke-RestMethod loripsum.net/api/10/long/plaintext -OutFile .\files\lorem$_.txt
}

PS ..\pwsh> (Get-Content .\files\lorem*.txt | Measure-Object Length -Sum).Sum / 1kb

86.787109375

PS ..\pwsh> (Compress-GzipArchive .\files\lorem*.txt -DestinationPath .\files\mergedLorem.gzip -PassThru).Length / 1kb

27.6982421875

PS ..\pwsh> (Expand-GzipArchive .\files\mergedLorem.gzip | Measure-Object Length -Sum).Sum / 1kb

86.775390625
```

### Example 10: Compressing the files content from previous example into one Gzip Base64 string

```powershell
PS ..\pwsh> (Get-Content .\files\lorem*.txt | ConvertTo-GzipString).Length / 1kb

36.94921875

PS ..\pwsh> (Get-Content .\files\lorem*.txt | ConvertTo-GzipString | ConvertFrom-GzipString -Raw).Length / 1kb

87.216796875
```
