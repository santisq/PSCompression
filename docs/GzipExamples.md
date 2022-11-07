## GZIP EXAMPLES

### Example 1: Strings to Gzip compressed Base 64 encoded string

```powershell
$strings = 'hello', 'world', '!'

# With positional binding
ConvertTo-GzipString $strings

# Or pipeline input, both work
$strings | ConvertTo-GzipString
```

### Example 2: Expanding compressed strings

```powershell
ConvertFrom-GzipString H4sIAAAAAAAACstIzcnJ5+Uqzy/KSeHlUuTlAgBLr/K2EQAAAA==
```

### Example 3: Demonstrates how `-NoNewLine` works

New lines are preserved when the function receives an array of strings

```powershell
$strings | ConvertTo-GzipString | ConvertFrom-GzipString
# When using the switch, all strings are concatenated
$strings | ConvertTo-GzipString -NoNewLine | ConvertFrom-GzipString
```

### Example 4: Create a Gzip compressed file from a string

Demonstrates how `-Raw` works

```powershell
'hello world!' | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip
```

### Example 5: Append content to the previous Gzip file

Demonstrates how `-Update` works

```powershell
'this is new content...' | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip -Update
```

### Example 6: Replace the previous Gzip file with new content

Demonstrates how `-Force` works

```powershell
$lorem = Invoke-RestMethod loripsum.net/api/10/long/plaintext
$lorem | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip -Force
```

### Example 7: Expanding a Gzip file

Output goes to the Success Stream when `-DestinationPath` is not bound

```powershell
Expand-GzipArchive .\files\file.gzip
```

### Example 8: Expanding a Gzip file outputting to a file

```powershell
Expand-GzipArchive .\files\file.gzip -DestinationPath .\files\file.txt

# Check integrity
$lorem | Set-Content .\files\strings.txt
Get-FileHash -Path .\files\file.txt, .\files\strings.txt -Algorithm MD5
```

### Example 9: Compressing multiple files into one Gzip file

Due to the nature of Gzip without Tar, all file contents are merged into one file

```powershell
0..10 | ForEach-Object {
    Invoke-RestMethod loripsum.net/api/10/long/plaintext -OutFile .\files\lorem$_.txt
}

(Get-Content .\files\lorem*.txt | Measure-Object Length -Sum).Sum / 1kb                                    # About 90kb
(Compress-GzipArchive .\files\lorem*.txt -DestinationPath .\files\mergedLorem.gzip -PassThru).Length / 1kb # About 30kb
(Expand-GzipArchive .\files\mergedLorem.gzip -Raw).Length / 1kb                                            # About 90kb
```

### Example 10: Compressing the files content from previous example into one Gzip Base64 string

```powershell
(Get-Content .\files\lorem*.txt | ConvertTo-GzipString).Length / 1kb                               # About 40kb
(Get-Content .\files\lorem*.txt | ConvertTo-GzipString | ConvertFrom-GzipString -Raw).Length / 1kb # About 90kb
```
