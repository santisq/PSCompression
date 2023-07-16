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
Compress-GzipArchive [-Path] <String[]> [-Destination] <String> [-CompressionLevel <CompressionLevel>]
 [-Update] [-Force] [-PassThru] [<CommonParameters>]
```

### LiteralPath

```powershell
Compress-GzipArchive -LiteralPath <String[]> [-Destination] <String> [-CompressionLevel <CompressionLevel>]
 [-Update] [-Force] [-PassThru] [<CommonParameters>]
```

### InputBytes

```powershell
Compress-GzipArchive -InputBytes <Byte[]> [-Destination] <String> [-CompressionLevel <CompressionLevel>]
 [-Update] [-Force] [-PassThru] [<CommonParameters>]
```

## DESCRIPTION

The `Compress-GzipArchive` cmdlet can compress one or more specified file paths into a single Gzip archive using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream). For expansion see [`Expand-GzipArchive`](Expand-ZipEntry.md).

## EXAMPLES

### Example 1: Create a Gzip compressed file from a File Path

```powershell
PS ..\pwsh> Compress-GzipArchive path\to\myFile.ext -Destination myFile.gz
```

If the destination does not end with `.gz` the extension is automatically added.

### Example 2: Create a Gzip compressed file from a string

```powershell
PS ..\pwsh> 'hello world!' | ConvertTo-GzipString -AsByteStream |
    Compress-GzipArchive -Destination .\files\file.gz
```

Demonstrates how `-AsByteStream` works on `ConvertTo-GzipString`.
Sends the compressed bytes to `Compress-GzipArchive`.

### Example 3: Append content to a Gzip archive

```powershell
PS ..\pwsh> 'this is new content...' | ConvertTo-GzipString -AsByteStream |
    Compress-GzipArchive -Destination .\files\file.gz -Update
```

Demonstrates how `-Update` works.

### Example 4: Replace a Gzip archive with new content

```powershell
PS ..\pwsh> $lorem = Invoke-RestMethod loripsum.net/api/10/long/plaintext
PS ..\pwsh> $lorem | ConvertTo-GzipString -AsByteStream |
    Compress-GzipArchive -Destination .\files\file.gz -Force
```

Demonstrates how `-Force` works.

### Example 5: Compressing multiple files into one Gzip archive

```powershell
PS ..\pwsh> 0..10 | ForEach-Object {
    Invoke-RestMethod loripsum.net/api/10/long/plaintext -OutFile .\files\lorem$_.txt
}

# Check the total Length of the downloaded files
PS ..\pwsh> (Get-Content .\files\lorem*.txt | Measure-Object Length -Sum).Sum / 1kb
86.787109375

# Check the total Length after compression
PS ..\pwsh> (Compress-GzipArchive .\files\lorem*.txt -Destination .\files\mergedLorem.gz -PassThru).Length / 1kb
27.6982421875
```

Due to the nature of Gzip without Tar, all file contents are merged into a single file.

## PARAMETERS

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

### -Destination

The path where to store the Gzip compressed file. The parent directory is created if it does not exist.

> __NOTE:__ If the path does not end with `.gz`, the cmdlet appends the `.gz` file name extension.

```yaml
Type: String
Parameter Sets: (All)
Aliases: DestinationPath

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force

Overwrites the Gzip archive if exists, otherwise it creates it.

> __NOTE:__ If `-Force` and `-Update` are used together this cmdlet will append content to the destination file.

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

### -InputBytes

This cmdlet can take input bytes from pipeline to create the output `.gz` archive file.

> __NOTE:__ This parameter is meant to be used exclusively in combination with `ConvertTo-GzipString -AsByteStream`.

```yaml
Type: Byte[]
Parameter Sets: InputBytes
Aliases:

Required: True
Position: Named
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
Parameter Sets: LiteralPath
Aliases: PSPath

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
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

### -Path

Specifies the path or paths to the files that you want to add to the Gzip archive file.
To specify multiple paths, and include files in multiple locations, use commas to separate the paths.
This Parameter accepts wildcard characters.
Wildcard characters allow you to add all files in a directory to your archive file.

```yaml
Type: String[]
Parameter Sets: Path
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: True
```

### -Update

Appends content to the existing Gzip file if exists, otherwise it creates it.

> __NOTE:__ If `-Force` and `-Update` are used together this cmdlet will append content to the destination file.

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

## INPUTS

### String

You can pipe paths to this cmdlet. Output from `Get-ChildItem` or `Get-Item` can be piped to this cmdlet.

## OUTPUTS

### None

By default, this cmdlet produces no output.

### FileInfo

When the `-PassThru` switch is used this cmdlet outputs the `FileInfo` instance representing the compressed file.
