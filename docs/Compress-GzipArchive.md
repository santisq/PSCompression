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

__All Gzip Examples can be found [here](/docs/GzipExamples.md).__

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
