---
external help file: PSCompression-help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Compress-GzipArchive

## SYNOPSIS

Creates a Gzip compressed file from specified File Paths or input Bytes.

## SYNTAX

### Path (Default)

```powershell
Compress-GzipArchive [-Path] <String[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-PassThru] [<CommonParameters>]
```

### PathWithForce

```powershell
Compress-GzipArchive [-Path] <String[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-Force] [-PassThru] [<CommonParameters>]
```

### PathWithUpdate

```powershell
Compress-GzipArchive [-Path] <String[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-Update] [-PassThru] [<CommonParameters>]
```

### LiteralPath

```powershell
Compress-GzipArchive -LiteralPath <String[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-PassThru] [<CommonParameters>]
```

### LiteralPathWithForce

```powershell
Compress-GzipArchive -LiteralPath <String[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-Force] [-PassThru] [<CommonParameters>]
```

### LiteralPathWithUpdate

```powershell
Compress-GzipArchive -LiteralPath <String[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-Update] [-PassThru] [<CommonParameters>]
```

### RawBytes

```powershell
Compress-GzipArchive -InputBytes <Byte[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-PassThru] [<CommonParameters>]
```

### RawBytesWithForce

```powershell
Compress-GzipArchive -InputBytes <Byte[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-Force] [-PassThru] [<CommonParameters>]
```

### RawBytesWithUpdate

```powershell
Compress-GzipArchive -InputBytes <Byte[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-Update] [-PassThru] [<CommonParameters>]
```

## DESCRIPTION

PowerShell function aimed to compress multiple files into a single Gzip file using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream). For expansion of Gzip files, see [`Expand-GzipArchive`](/docs/Expand-GzipArchive.md)

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
# New lines are preserved when the function receives an array of strings.
PS ..\pwsh> $strings | ConvertTo-GzipString | ConvertFrom-GzipString

hello
world
!

# When using the `-NoNewLine` switch, all strings are concatenated
PS ..\pwsh> $strings | ConvertTo-GzipString -NoNewLine | ConvertFrom-GzipString

helloworld!
```

### Example 4: Create a Gzip compressed file from a string

```powershell
# Demonstrates how `-Raw` works on `ConvertTo-GzipString`.
# Sends the compressed bytes to `Compress-GzipArchive`.
PS ..\pwsh> 'hello world!' | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip
```

### Example 5: Append content to the previous Gzip file

```powershell
# Demonstrates how `-Update` works.
PS ..\pwsh> 'this is new content...' | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip -Update
```

### Example 6: Expanding a Gzip file

```powershell
# Output goes to the Success Stream when `-DestinationPath` is not bound.
PS ..\pwsh> Expand-GzipArchive .\files\file.gzip

hello world!
this is new content...
```

### Example 7: Replace the previous Gzip file with new content

```powershell
# Demonstrates how `-Force` works.
PS ..\pwsh> $lorem = Invoke-RestMethod loripsum.net/api/10/long/plaintext
PS ..\pwsh> $lorem | ConvertTo-GzipString -Raw |
    Compress-GzipArchive -DestinationPath .\files\file.gzip -Force
```

### Example 8: Expanding a Gzip file outputting to a file

```powershell
PS ..\pwsh> Expand-GzipArchive .\files\file.gzip -DestinationPath .\files\file.txt

# Checking Length Difference
PS ..\pwsh> Get-Item -Path .\files\file.gzip, .\files\file.txt |
    Select-Object Name, Length

Name      Length
----      ------
file.gzip   3168
file.txt    6857

# Checking Integrity between expanded and original
PS ..\pwsh> $lorem | Set-Content .\files\strings.txt
PS ..\pwsh> Get-FileHash -Path .\files\file.txt, .\files\strings.txt -Algorithm MD5

Hash
----
E22E4786F9666E6E7F4A512B3714C517
E22E4786F9666E6E7F4A512B3714C517
```

### Example 9: Compressing multiple files into one Gzip file

```powershell
# Due to the nature of Gzip without Tar, all file contents are merged into one file.
PS ..\pwsh> 0..10 | ForEach-Object {
    Invoke-RestMethod loripsum.net/api/10/long/plaintext -OutFile .\files\lorem$_.txt
}

# Check the total Length of the downloaded files
PS ..\pwsh> (Get-Content .\files\lorem*.txt | Measure-Object Length -Sum).Sum / 1kb

86.787109375

# Check the total Length after Gzip compression
PS ..\pwsh> (Compress-GzipArchive .\files\lorem*.txt -DestinationPath .\files\mergedLorem.gzip -PassThru).Length / 1kb

27.6982421875

# Expand the compressed file
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

## PARAMETERS

### -Path

Specifies the path or paths to the files that you want to add to the Gzip archive file.
To specify multiple paths, and include files in multiple locations, use commas to separate the paths.
This Parameter accepts wildcard characters.
Wildcard characters allow you to add all files in a directory to your archive file.

```yaml
Type: String[]
Parameter Sets: Path, PathWithForce, PathWithUpdate
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -LiteralPath

Specifies the path or paths to the files that you want to add to the Gzip archive file.
Unlike the `-Path` Parameter, the value of `-LiteralPath` is used exactly as it's typed.
No characters are interpreted as wildcards

```yaml
Type: String[]
Parameter Sets: LiteralPath, LiteralPathWithForce, LiteralPathWithUpdate
Aliases: PSPath

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -InputBytes

Takes the bytes from pipeline and adds to the Gzip archive file.
This parameter is meant to be used in combination with [`ConvertTo-GzipString -Raw`](/docs/ConvertTo-GzipString.md).

```yaml
Type: Byte[]
Parameter Sets: RawBytes, RawBytesWithForce, RawBytesWithUpdate
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -DestinationPath

The destination path to the Gzip file.
If the file name in DestinationPath doesn't have a `.gzip` file name extension, the function appends the `.gzip` file name extension.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 2
Default value: None
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
Position: Named
Default value: Optimal
Accept pipeline input: False
Accept wildcard characters: False
```

### -Update

Appends to the existing Gzip file.

```yaml
Type: SwitchParameter
Parameter Sets: PathWithUpdate, LiteralPathWithUpdate, RawBytesWithUpdate
Aliases:

Required: True
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force

Replaces an existing Gzip file with a new one.
All contents will be lost.

```yaml
Type: SwitchParameter
Parameter Sets: PathWithForce, LiteralPathWithForce, RawBytesWithForce
Aliases:

Required: True
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru

Outputs the object representing the compressed file.
The function produces no output by default.

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
