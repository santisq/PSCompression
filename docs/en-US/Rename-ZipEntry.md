---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version:
schema: 2.0.0
---

# Rename-ZipEntry

## SYNOPSIS

Renames Zip Archive Entries from one or more Zip Archives.

## SYNTAX

```powershell
Rename-ZipEntry
    [-ZipEntry] <ZipEntryBase>
    [-NewName] <String>
    [-PassThru]
    [-WhatIf]
    [-Confirm]
    [<CommonParameters>]
```

## DESCRIPTION

The `Rename-ZipEntry` cmdlet changes the name of a specified item.
This cmdlet does not affect the content of the item being renamed.

> [!NOTE]
>
> - It's important to note that there is no API in the
[`System.IO.Compression` Namespace](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression) to rename Zip
Archive Entries, renaming an entry means creating a copy of the entry with a new name and deleting the old one.
__This is why the renaming operations can be slow on big Zip Archives.__
> - When renaming an entry that is of type `Directory` you need to consider that the operation explained in the previous
point happens to every entry that is considered a child of the directory you are renaming.

## EXAMPLES

### Example 1

```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -NewName

{{ Fill NewName Description }}

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -PassThru

{{ Fill PassThru Description }}

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

### -ZipEntry

{{ Fill ZipEntry Description }}

```yaml
Type: ZipEntryBase
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Confirm

Prompts you for confirmation before running the cmdlet.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: cf

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -WhatIf

Shows what would happen if the cmdlet runs.
The cmdlet is not run.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: wi

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### PSCompression.ZipEntryBase

### System.String

## OUTPUTS

### PSCompression.ZipEntryFile

### PSCompression.ZipEntryDirectory

## NOTES

## RELATED LINKS
