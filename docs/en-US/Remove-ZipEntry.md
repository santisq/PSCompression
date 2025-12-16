---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Remove-ZipEntry

## SYNOPSIS

Removes specified entries from zip archives.

## SYNTAX

```powershell
Remove-ZipEntry
    -InputObject <ZipEntryBase[]>
    [-WhatIf]
    [-Confirm]
    [<CommonParameters>]
```

## DESCRIPTION

The `Remove-ZipEntry` cmdlet removes `ZipEntryFile` or `ZipEntryDirectory` objects produced by [`Get-ZipEntry`](./Get-ZipEntry.md) or [`New-ZipEntry`](./New-ZipEntry.md) from their parent zip archives.

## EXAMPLES

### Example 1: Remove all Zip Archive Entries from a Zip Archive

```powershell
PS ..pwsh\> Get-ZipEntry .\myZip.zip | Remove-ZipEntry
```

This example removes all entries from `myZip.zip`, effectively emptying the archive.

### Example 2: Remove all `.txt` Entries from a Zip Archive

```powershell
PS ..pwsh\> Get-ZipEntry .\myZip.zip -Include *.txt | Remove-ZipEntry
```

This example removes only the entries matching the `*.txt` pattern from `myZip.zip`.

### Example 3: Prompt for confirmation before removing entries

```powershell
PS ..pwsh\> Get-ZipEntry .\myZip.zip -Include *.txt | Remove-ZipEntry -Confirm

Confirm
Are you sure you want to perform this action?
Performing the operation "Remove" on target "test/helloworld.txt".
[Y] Yes  [A] Yes to All  [N] No  [L] No to All  [S] Suspend  [?] Help (default is "Y"):
```

This example prompts for confirmation before removing each matching entry. The cmdlet supports [`ShouldProcess`](https://learn.microsoft.com/en-us/powershell/scripting/learn/deep-dives/everything-about-shouldprocess): use `-Confirm` to prompt or `-WhatIf` to preview changes without applying them.

## PARAMETERS

### -InputObject

Specifies the zip entries to remove. This parameter accepts pipeline input from `Get-ZipEntry` or `New-ZipEntry` and can also be used as a named argument.

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

Shows what would happen if the cmdlet runs. The cmdlet is not run.

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

### PSCompression.Abstractions.ZipEntryBase[]

You can pipe one or more `ZipEntryFile` or `ZipEntryDirectory` objects produced by [`Get-ZipEntry`](./Get-ZipEntry.md) or [`New-ZipEntry`](./New-ZipEntry.md) to this cmdlet.

## OUTPUTS

### None

This cmdlet produces no output.

## NOTES

## RELATED LINKS

[__`Get-ZipEntry`__](./Get-ZipEntry.md)

[__`New-ZipEntry`__](./New-ZipEntry.md)

[__System.IO.Compression.ZipArchive__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive)

[__System.IO.Compression.ZipArchiveEntry__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchiveentry)
