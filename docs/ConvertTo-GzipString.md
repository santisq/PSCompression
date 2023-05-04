---
external help file: PSCompression-help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# ConvertTo-GzipString

## SYNOPSIS

Creates a Base64 Gzip compressed string from a specified input string or strings.

## SYNTAX

```powershell
ConvertTo-GzipString [-InputObject] <String[]> [[-Encoding] <Encoding>]
 [[-CompressionLevel] <CompressionLevel>] [-Raw] [-NoNewLine] [<CommonParameters>]
```

## DESCRIPTION

PowerShell function aimed to compress input strings into Base64 encoded Gzip strings or raw bytes using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream). For expansion of Base64 Gzip strings, see [`ConvertFrom-GzipString`](/docs/ConvertFrom-GzipString.md).

## EXAMPLES

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

```powershell
PS ..\pwsh> $strings | ConvertTo-GzipString | ConvertFrom-GzipString

hello
world
!

# When using the switch, all strings are concatenated
PS ..\pwsh> $strings | ConvertTo-GzipString -NoNewLine | ConvertFrom-GzipString

helloworld!
```

New lines are preserved when the function receives an array of strings.

### Example 4: Create a Gzip compressed file from a string

```powershell
PS ..\pwsh> 'hello world!' | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip
```

Demonstrates how `-Raw` works on `ConvertTo-GzipString`. Sends the compressed bytes to `Compress-GzipArchive`.

### Example 5: Append content to the previous Gzip file

```powershell
PS ..\pwsh> 'this is new content...' | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip -Update
```

Demonstrates how `-Update` works.

### Example 6: Expanding a Gzip file

```powershell
PS ..\pwsh> Expand-GzipArchive .\files\file.gzip

hello world!
this is new content...
```

Output goes to the Success Stream when `-DestinationPath` is not bound.

### Example 7: Replace the previous Gzip file with new content

```powershell
PS ..\pwsh> $lorem = Invoke-RestMethod loripsum.net/api/10/long/plaintext
PS ..\pwsh> $lorem | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip -Force
```

Demonstrates how `-Force` works.

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

Due to the nature of Gzip without Tar, all file contents are merged into one file.

### Example 10: Compressing the files content from previous example into one Gzip Base64 string

```powershell
PS ..\pwsh> (Get-Content .\files\lorem*.txt | ConvertTo-GzipString).Length / 1kb

36.94921875

PS ..\pwsh> (Get-Content .\files\lorem*.txt | ConvertTo-GzipString | ConvertFrom-GzipString -Raw).Length / 1kb

87.216796875
```

## PARAMETERS

### -InputObject

Specifies the input string or strings to compress.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Encoding

Character encoding used when compressing the Gzip input.

```yaml
Type: Encoding
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: Utf-8
Accept pipeline input: False
Accept wildcard characters: False
```

### -CompressionLevel

Define the compression level that should be used.
See [`CompressionLevel` Enum](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.compressionlevel) for details.

```yaml
Type: CompressionLevel
Parameter Sets: (All)
Aliases:
Accepted values: Optimal, Fastest, NoCompression, SmallestSize

Required: False
Position: 3
Default value: Optimal
Accept pipeline input: False
Accept wildcard characters: False
```

### -Raw

Outputs the compressed bytes to the Success Stream.
There is no Base64 Encoding when this parameter is used.
This parameter is meant to be used in combination with [`Compress-GzipArchive`](/docs/Compress-GzipArchive.md).

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -NoNewLine

The encoded string representations of the input objects are concatenated to form the output.
No spaces or newlines are inserted between the output strings.
No newline is added after the last output string.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).
