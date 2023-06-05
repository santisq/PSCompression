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
PS ..\pwsh> Get-ZipEntry path\to\myZip.zip -EntryType Archive
```

The `-EntryType` parameter supports filtering by `Archive` or `Directory`.

### Example 4: Filtering entries with `-Include` and `-Exclude` parameters

```powershell
PS ..\pwsh> Get-ZipEntry .\test.zip -Include docs/en-us*

   Directory: docs/en-US/

EntryType               LastWriteTime  CompressedSize            Size EntryName
---------               -------------  --------------            ---- ---------
Directory          5/30/2023 12:34 AM         0.00  B         0.00  B
Archive            5/30/2023 12:32 AM         1.92 KB         7.58 KB Compress-GzipArchive.md
Archive            5/30/2023 12:33 AM         1.99 KB         7.26 KB Compress-ZipArchive.md
Archive            5/30/2023 12:32 AM         1.02 KB         2.52 KB ConvertFrom-GzipString.md
Archive            5/30/2023 12:32 AM         1.53 KB         4.36 KB ConvertTo-GzipString.md
Archive            5/30/2023 12:32 AM         1.47 KB         4.77 KB Expand-GzipArchive.md
Archive            5/30/2023 12:13 PM         1.20 KB         3.89 KB Expand-ZipEntry.md
Archive            5/31/2023 10:54 AM         1.02 KB         3.29 KB Get-ZipEntry.md
Archive            5/29/2023  3:49 PM       800.00  B         2.69 KB Get-ZipEntryContent.md
Archive            5/29/2023  3:49 PM       736.00  B         1.83 KB New-ZipEntry.md
Archive            5/29/2023  3:11 PM       411.00  B         1.30 KB PSCompression.md
Archive            5/29/2023  3:49 PM       643.00  B         1.19 KB Remove-ZipEntry.md
Archive            5/29/2023  3:49 PM       842.00  B         2.88 KB Set-ZipEntryContent.md

PS ..\pwsh> Get-ZipEntry .\test.zip -Include docs/en-us* -Exclude *Compress*, *Remove*

   Directory: docs/en-US/

EntryType               LastWriteTime  CompressedSize            Size EntryName
---------               -------------  --------------            ---- ---------
Directory          5/30/2023 12:34 AM         0.00  B         0.00  B
Archive            5/30/2023 12:32 AM         1.02 KB         2.52 KB ConvertFrom-GzipString.md
Archive            5/30/2023 12:32 AM         1.53 KB         4.36 KB ConvertTo-GzipString.md
Archive            5/30/2023 12:32 AM         1.47 KB         4.77 KB Expand-GzipArchive.md
Archive            5/30/2023 12:13 PM         1.20 KB         3.89 KB Expand-ZipEntry.md
Archive            5/31/2023 10:54 AM         1.02 KB         3.29 KB Get-ZipEntry.md
Archive            5/29/2023  3:49 PM       800.00  B         2.69 KB Get-ZipEntryContent.md
Archive            5/29/2023  3:49 PM       736.00  B         1.83 KB New-ZipEntry.md
Archive            5/29/2023  3:49 PM       842.00  B         2.88 KB Set-ZipEntryContent.md
```

Inclusion and Exclusion patterns are applied to the entries relative path.
Exclusions are applied after the inclusions.

## PARAMETERS

### -EntryType

Lists entries of a specified type, `Archive` or `Directory`.

```yaml
Type: ZipEntryType
Parameter Sets: (All)
Aliases:
Accepted values: Directory, Archive

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Exclude

Specifies an array of one or more string patterns to be matched as the cmdlet lists entries. Any matching item is excluded from the output. Wildcard characters are accepted.

__Inclusion and Exclusion patterns are applied to the entries relative path.
Exclusions are applied after the inclusions.__

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

Specifies an array of one or more string patterns to be matched as the cmdlet lists entries. Any matching item is included in the output. Wildcard characters are accepted.

__Inclusion and Exclusion patterns are applied to the entries relative path.
Exclusions are applied after the inclusions.__

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

Specifies a path to one or more Zip compressed files. Note that the value is used exactly as it's typed. No characters are interpreted as wildcards.

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

Specifies a path to one or more Zip compressed files. Wildcards are accepted.

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

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### String[]

## OUTPUTS

### ZipEntryDirectory

### ZipEntryFile
