---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version:
schema: 2.0.0
---

# Rename-ZipEntry

## SYNOPSIS

{{ Fill in the Synopsis }}

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

{{ Fill in the Description }}

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
