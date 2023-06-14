---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Remove-ZipEntry

## SYNOPSIS

Removes Zip Archive Entries from one or more Zip Archives.

## SYNTAX

```powershell
Remove-ZipEntry -InputObject <ZipEntryBase[]> [-WhatIf] [-Confirm] [<CommonParameters>]
```

## DESCRIPTION

The `Remove-ZipEntry` cmdlet can remove Zip Archive Entries from one or more Zip Archives. This cmdlet takes input from and is intended to be used in combination with the `Get-ZipEntry` cmdlet.

## EXAMPLES

### Example 1: Remove all Zip Archive Entries from a Zip Archive

```powershell
PS ..pwsh\> Get-ZipEntry .\myZip.zip | Remove-ZipEntry
```

### Example 2: Remove all `.txt` Entries from a Zip Archive

```powershell
PS ..pwsh\> Get-ZipEntry .\myZip.zip -Include *.txt | Remove-ZipEntry
```

### Example 3: Prompt for confirmation before removing entries

```powershell
PS ..pwsh\> Get-ZipEntry .\myZip.zip -Include *.txt | Remove-ZipEntry -Confirm

Confirm
Are you sure you want to perform this action?
Performing the operation "Remove" on target "test/helloworld.txt".
[Y] Yes  [A] Yes to All  [N] No  [L] No to All  [S] Suspend  [?] Help (default is "Y"):
```

This cmdlet supports [`ShouldProcess`](https://learn.microsoft.com/en-us/powershell/scripting/learn/deep-dives/everything-about-shouldprocess?view=powershell-7.3), you can prompt for confirmation before removing entries with `-Confirm` or check what the cmdlet would do without performing any action with `-WhatIf`.

## PARAMETERS

### -InputObject

The entries that should be removed. This parameter can be and is meant to be bound from pipeline however can be also used as a named parameter.

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

### ZipEntryBase[]

## OUTPUTS

### None

This cmdlet produces no output.
