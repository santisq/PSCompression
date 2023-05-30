---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Get-ZipEntry

## SYNOPSIS

Lists Zip Archive Entries from one or more specified Zip Archive paths.

## SYNTAX

### Path (Default)

```powershell
Get-ZipEntry [-Path] <String[]> [-EntryType <String>] [-Include <String[]>] [-Exclude <String[]>]
 [<CommonParameters>]
```

### LiteralPath

```powershell
Get-ZipEntry -LiteralPath <String[]> [-EntryType <String>] [-Include <String[]>] [-Exclude <String[]>]
 [<CommonParameters>]
```

## DESCRIPTION

The `Get-ZipEntry` cmdlet lists entries from specified Zip paths. It has built-in functionalities to filter entries and is the main entry point for the ZipEntry cmdlets in this module.

## EXAMPLES

### Example 1: List entries for a specified Zip file path

```powershell
PS ..\pwsh> Get-ZipEntry path\to\myZip.zip
```

### Example 2: List entries from all Zip files in the current directory

```powershell
PS ..\pwsh> Get-ZipEntry *.zip
```

The `-Path` parameter supports wildcards.

### Example 3: List all `Archive` entries from a Zip file

```powershell
PS ..pwsh\> Get-ZipEntry path\to\myZip.zip -EntryType Archive
```

The `-EntryType` parameter supports filtering by `Archive` or `Directory`.

### Example 1

```powershell
PS ..pwsh\>
```

### Example 1

```powershell
PS ..pwsh\>
```

{{ Add example description here }}

## PARAMETERS

### -EntryType

{{ Fill EntryType Description }}

```yaml
Type: String
Parameter Sets: (All)
Aliases:
Accepted values: File, Directory

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Exclude

{{ Fill Exclude Description }}

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -Include

{{ Fill Include Description }}

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -LiteralPath

{{ Fill LiteralPath Description }}

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

### -Path

{{ Fill Path Description }}

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

### CommonParameters

This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

## OUTPUTS

### PSCompression.ZipEntryDirectory

### PSCompression.ZipEntryDirectory

## NOTES

## RELATED LINKS
