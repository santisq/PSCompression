---
external help file: PSCompression-help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Expand-GzipArchive

## SYNOPSIS
Expands a Gzip compressed file from a specified File Path or Paths.

## SYNTAX

### Path (Default)
```powershell
Expand-GzipArchive [-Path] <String[]> [-Encoding <Encoding>] [<CommonParameters>]
```

### PathToFile
```powershell
Expand-GzipArchive [-Path] <String[]> [[-DestinationPath] <String>] [-PassThru] [<CommonParameters>]
```

### LiteralPath
```powershell
Expand-GzipArchive -LiteralPath <String[]> [-Encoding <Encoding>] [<CommonParameters>]
```

### LiteralPathToFile
```powershell
Expand-GzipArchive -LiteralPath <String[]> [[-DestinationPath] <String>] [-PassThru] [<CommonParameters>]
```

## DESCRIPTION
PowerShell function aimed to expand Gzip files into a using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream). This function is the counterpart of [`Compress-GzipArchive`](/docs/Compress-GzipArchive.md).

## EXAMPLES
__All Gzip Examples can be found [here](/docs/GzipExamples.md).__

## PARAMETERS

### -Path
Specifies the path or paths to the Gzip files to expand.
To specify multiple paths, and include files in multiple locations, use commas to separate the paths.
This Parameter accepts wildcard characters.
Wildcard characters allow you to add all files in a directory to your archive file.

```yaml
Type: String[]
Parameter Sets: Path, PathToFile
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -LiteralPath
Specifies the path or paths to the Gzip files to expand.
Unlike the `-Path` Parameter, the value of `-LiteralPath` is used exactly as it's typed.
No characters are interpreted as wildcards

```yaml
Type: String[]
Parameter Sets: LiteralPath, LiteralPathToFile
Aliases: PSPath

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -DestinationPath
The destination path to where to expand the Gzip file.
The target folder is created if it does not exist.
This parameter is Optional, if not used, this function outputs to the Success Stream.

```yaml
Type: String
Parameter Sets: PathToFile, LiteralPathToFile
Aliases:

Required: False
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Encoding
Character encoding used when expanding the Gzip content.
This parameter is only available when expanding to the Success Stream.

```yaml
Type: Encoding
Parameter Sets: Path, LiteralPath
Aliases:

Required: False
Position: Named
Default value: Utf8
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru
Outputs the object representing the expanded file.
This parameter is only available when expanding to a File.

```yaml
Type: SwitchParameter
Parameter Sets: PathToFile, LiteralPathToFile
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).
