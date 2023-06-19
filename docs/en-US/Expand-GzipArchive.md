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

### Path

```powershell
Expand-GzipArchive [-Path] <String[]> [-Encoding <Encoding>] [-Raw] [<CommonParameters>]
```

### LiteralPath

```powershell
Expand-GzipArchive -LiteralPath <String[]> [-Encoding <Encoding>] [-Raw] [<CommonParameters>]
```

### PathDestination

```powershell
Expand-GzipArchive [-Path] <String[]> [-DestinationPath] <String> [-PassThru] [<CommonParameters>]
```

### LiteralPathDestination

```powershell
Expand-GzipArchive -LiteralPath <String[]> [-DestinationPath] <String> [-PassThru] [<CommonParameters>]
```

## DESCRIPTION

The `Expand-GzipArchive` cmdlet aims to expand Gzip compressed files to a destination path or to the success stream using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream). This cmdlet is the counterpart of [`Compress-GzipArchive`](Compress-GzipArchive.md).

## EXAMPLES

### Example 1: Expanding a Gzip archive to the success stream

```powershell
PS ..\pwsh> Expand-GzipArchive .\files\file.gz

hello world!
```

Output goes to the Success Stream when `-DestinationPath` is not used.

### Example 2: Expanding a Gzip archive to a new file

```powershell
PS ..\pwsh> Expand-GzipArchive .\files\file.gzip -DestinationPath .\files\file.txt

# Checking Length Difference
PS ..\pwsh> Get-Item -Path .\files\file.gzip, .\files\file.txt |
    Select-Object Name, Length

Name      Length
----      ------
file.gzip   3168
file.txt    6857
```

## PARAMETERS

### -Path

Specifies the path or paths to the Gzip files to expand.
To specify multiple paths, and include files in multiple locations, use commas to separate the paths.
This Parameter accepts wildcard characters.
Wildcard characters allow you to add all files in a directory to your archive file.

```yaml
Type: String[]
Parameter Sets: PathDestination, Path
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
Parameter Sets: LiteralPathDestination, LiteralPath
Aliases: PSPath

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -DestinationPath

The destination path where to expand the Gzip file.
The target folder is created if it does not exist.
This parameter is Optional, if not used, this cmdlet outputs to the Success Stream.

```yaml
Type: String
Parameter Sets: PathDestination, LiteralPathDestination
Aliases:

Required: True
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Encoding

Character encoding used when expanding the Gzip content. This parameter is only available when expanding to the Success Stream. The default encoding is __`utf8NoBOM`.__

```yaml
Type: Encoding
Parameter Sets: Path, LiteralPath
Aliases:

Required: False
Position: Named
Default value: utf8NoBOM
Accept pipeline input: False
Accept wildcard characters: False
```

### -Raw

Outputs the expanded file as a single string with newlines preserved.
By default, newline characters in the expanded string are used as delimiters to separate the input into an array of strings.
This parameter is only available when expanding to the Success Stream.

```yaml
Type: SwitchParameter
Parameter Sets: Path, LiteralPath
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru

Outputs the object representing the expanded file.
This parameter is only available when expanding to a File.

```yaml
Type: SwitchParameter
Parameter Sets: PathDestination, LiteralPathDestination
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

### String

This cmdlet outputs an array of string to the success stream when `-DestinationPath` is not used and a single multi-line string when used with the `-Raw` switch.

### None

This cmdlet produces no output when expanding to a file and `-PassThru` is not used.

### FileInfo

When the `-PassThru` switch is used this cmdlet outputs the `FileInfo` instance representing the expanded file.
