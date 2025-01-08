---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Expand-ZipEntry

## SYNOPSIS

Expands Zip Archive Entries to a destination directory.

## SYNTAX

```powershell
Expand-ZipEntry
    -InputObject <ZipEntryBase[]>
    [-Destination <String>]
    [-Force]
    [-PassThru]
    [<CommonParameters>]
```

## DESCRIPTION

The `Expand-ZipEntry` cmdlet can expand zip entries outputted by the [`Get-ZipEntry`](./Get-ZipEntry.md) command to a destination directory. Expanded entries maintain their original folder structure based on their relative path.

## EXAMPLES

### Example 1: Extract all `.txt` files from a Zip Archive to the current directory

```powershell
PS ..\pwsh> Get-ZipEntry path\to\myZip.zip -Include *.txt | Expand-ZipEntry
```

### Example 2: Extract all `.txt` files from a Zip Archive to the a desired directory

```powershell
PS ..\pwsh> Get-ZipEntry path\to\myZip.zip -Include *.txt | Expand-ZipEntry -Destination path\to\myfolder
```

### Example 3: Extract all entries excluding `.txt` files to the current directory

```powershell
PS ..\pwsh> Get-ZipEntry path\to\myZip.zip -Exclude *.txt | Expand-ZipEntry
```

### Example 4: Extract all entries excluding `.txt` files to the current directory overwritting existing files

```powershell
PS ..\pwsh> Get-ZipEntry path\to\myZip.zip -Exclude *.txt | Expand-ZipEntry -Force
```

Demonstrates how `-Force` switch works.

### Example 5: Extract all entries excluding `.txt` files to the current directory outputting the expanded entries

```powershell
PS ..\pwsh> Get-ZipEntry path\to\myZip.zip -Exclude *.txt | Expand-ZipEntry -PassThru
```

By default this cmdlet produces no output. When `-PassThru` is used, this cmdlet outputs the `FileInfo` and `DirectoryInfo` instances representing the expanded entries.

## PARAMETERS

### -Destination

The destination directory where to extract the Zip Entries.

> [!NOTE]
> This parameter is optional, when not used, the entries are extracted to the their relative zip path in the current directory.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: $PWD
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force

Existing files in the destination directory are overwritten when this switch is used.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru

The cmdlet outputs the `FileInfo` and `DirectoryInfo` instances representing the extracted entries when this switch is used.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -InputObject

The zip entries to expand.

> [!NOTE]
>
> - This parameter takes input from pipeline, however binding by name is also possible.
> - The input are instances inheriting from `ZipEntryBase` (`ZipEntryFile` or `ZipEntryDirectory`) outputted by [`Get-ZipEntry`](Get-ZipEntry.md) and [`New-ZipEntry`](New-ZipEntry.md) cmdlets.

```yaml
Type: ZipEntryBase[]
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### ZipEntryBase

You can pipe instances of `ZipEntryFile` or `ZipEntryDirectory` to this cmdlet. These instances are produced by [`Get-ZipEntry`](Get-ZipEntry.md) and [`New-ZipEntry`](New-ZipEntry.md) cmdlets.

## OUTPUTS

### None

By default, this cmdlet produces no output.

### FileSystemInfo

The cmdlet outputs the `FileInfo` and `DirectoryInfo` instances of the extracted entries when `-PassThru` switch is used.
