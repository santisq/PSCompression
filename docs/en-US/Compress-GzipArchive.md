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

PowerShell cmdlet aimed to compress multiple files into a single Gzip file using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream). For expansion see `Expand-GzipArchive`.

## EXAMPLES

### Example 1: Create a Gzip compressed file from a File Path

```powershell
# If the destination doesn't end with `.gz` the extension will be automatically added
PS ..\pwsh> Compress-GzipArchive path\to\myFile.ext -DestinationPath myFile.gz
```

### Example 2: Create a Gzip compressed file from a string

```powershell
# Demonstrates how `-AsByteStream` works on `ConvertTo-GzipString`.
# Sends the compressed bytes to `Compress-GzipArchive`.
PS ..\pwsh> 'hello world!' | ConvertTo-GzipString -AsByteStream |
    Compress-GzipArchive -DestinationPath .\files\file.gz
```

### Example 3: Append content to a Gzip file

```powershell
# Demonstrates how `-Update` works.
PS ..\pwsh> 'this is new content...' | ConvertTo-GzipString -AsByteStream |
    Compress-GzipArchive -DestinationPath .\files\file.gz -Update
```

### Example 4: Replace a Gzip file with new content

```powershell
# Demonstrates how `-Force` works.
PS ..\pwsh> $lorem = Invoke-RestMethod loripsum.net/api/10/long/plaintext
PS ..\pwsh> $lorem | ConvertTo-GzipString -AsByteStream |
    Compress-GzipArchive -DestinationPath .\files\file.gzip -Force
```

### Example 5: Compressing multiple files into one Gzip file

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
This parameter is meant to be used in combination with `ConvertTo-GzipString -AsByteStream`.

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

The destination path to the Gzip compressed file.
If the file name in DestinationPath doesn't have a `.gz` file name extension, the cmdlet appends the `.gz` file name extension.

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

Appends content to the existing Gzip file.

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

Replaces the content of a Gzip file.

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
The cmdlet produces no output by default.

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

## OUTPUTS

### None

By default, this cmdlet produces no output.

### System.IO.FileInfo

When the `-PassThru` switch is used this cmdlet outputs the `FileInfo` instance representing the compressed file.
