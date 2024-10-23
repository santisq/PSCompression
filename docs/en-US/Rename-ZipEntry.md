---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version:
schema: 2.0.0
---

# Rename-ZipEntry

## SYNOPSIS

Renames zip entries from one or more zip archives.

## SYNTAX

```powershell
Rename-ZipEntry
    -ZipEntry <ZipEntryBase>
    -NewName <String>
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

### Example 1: Rename a Zip File Entry

```powershell
PS ..pwsh\> Get-ZipEntry .\myZip.zip -Type Archive -Include relativePath/to/myEntryToRename.ext |
    Rename-ZipEntry -NewName myNewName.ext
```

### Example 2: Rename all entries with `.ext` extension using a delay-bind scriptblock

```powershell
PS ..pwsh\> Get-ZipEntry .\myZip.zip -Type Archive -Include *.ext |
    Rename-ZipEntry -NewName { $_.BaseName + 'myNewName' + $_.Extension }
```

[Delay-bind scriptblocks](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_script_blocks?view=powershell-7.4#using-delay-bind-script-blocks-with-parameters) is supported for renaming multiple entries.

> [!TIP]
> In the context of the _delay-bind scriptblock_, [`$_` (`$PSItem`)](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_psitem?view=powershell-7.4) represents the current pipeline item, in this case, an instance of `PSCompression.ZipEntryFile` or `PSCompression.ZipEntryDirectory`.

### Example 3: Rename a Zip Directory Entry

```powershell
PS ..\pwsh> Get-ZipEntry .\PSCompression.zip -Include PSCompression/docs/en-US/*

   Directory: /PSCompression/docs/en-US/

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Directory          2/22/2024  1:19 PM                                 en-US
Archive            2/22/2024  1:19 PM         2.08 KB         6.98 KB Compress-GzipArchive.md
Archive            2/22/2024  1:19 PM         2.74 KB         8.60 KB Compress-ZipArchive.md
Archive            2/22/2024  1:19 PM         1.08 KB         2.67 KB ConvertFrom-GzipString.md
Archive            2/22/2024  1:19 PM         1.67 KB         4.63 KB ConvertTo-GzipString.md
Archive            2/22/2024  1:19 PM         1.74 KB         6.28 KB Expand-GzipArchive.md
Archive            2/22/2024  1:19 PM         1.23 KB         4.07 KB Expand-ZipEntry.md
Archive            2/22/2024  1:19 PM         1.53 KB         6.38 KB Get-ZipEntry.md
Archive            2/22/2024  1:19 PM         1.67 KB         5.06 KB Get-ZipEntryContent.md
Archive            2/22/2024  1:19 PM         2.20 KB         7.35 KB New-ZipEntry.md
Archive            2/22/2024  1:19 PM       961.00  B         2.62 KB PSCompression.md
Archive            2/22/2024  1:19 PM         1.14 KB         2.95 KB Remove-ZipEntry.md
Archive            2/22/2024  1:19 PM       741.00  B         2.16 KB Rename-ZipEntry.md
Archive            2/22/2024  1:19 PM         1.55 KB         5.35 KB Set-ZipEntryContent.md

PS ..\pwsh> Get-ZipEntry .\PSCompression.zip -Include PSCompression/docs/en-US/ | Rename-ZipEntry -NewName 'en-US123'
PS ..\pwsh> Get-ZipEntry .\PSCompression.zip -Include PSCompression/docs/en-US123/*

   Directory: /PSCompression/docs/en-US123/

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Directory          2/25/2024 12:41 PM                                 en-US123
Archive            2/25/2024 12:41 PM         2.08 KB         6.98 KB Compress-GzipArchive.md
Archive            2/25/2024 12:41 PM         2.74 KB         8.60 KB Compress-ZipArchive.md
Archive            2/25/2024 12:41 PM         1.08 KB         2.67 KB ConvertFrom-GzipString.md
Archive            2/25/2024 12:41 PM         1.67 KB         4.63 KB ConvertTo-GzipString.md
Archive            2/25/2024 12:41 PM         1.74 KB         6.28 KB Expand-GzipArchive.md
Archive            2/25/2024 12:41 PM         1.23 KB         4.07 KB Expand-ZipEntry.md
Archive            2/25/2024 12:41 PM         1.53 KB         6.38 KB Get-ZipEntry.md
Archive            2/25/2024 12:41 PM         1.67 KB         5.06 KB Get-ZipEntryContent.md
Archive            2/25/2024 12:41 PM         2.20 KB         7.35 KB New-ZipEntry.md
Archive            2/25/2024 12:41 PM       961.00  B         2.62 KB PSCompression.md
Archive            2/25/2024 12:41 PM         1.14 KB         2.95 KB Remove-ZipEntry.md
Archive            2/25/2024 12:41 PM       741.00  B         2.16 KB Rename-ZipEntry.md
Archive            2/25/2024 12:41 PM         1.55 KB         5.35 KB Set-ZipEntryContent.md
```

### Example 4: Prompt for confirmation before renaming entries

```powershell
PS ..pwsh\> Get-ZipEntry .\PSCompression.zip -Include PSCompression/docs/en-US123/ | Rename-ZipEntry -NewName 'Test' -Confirm

Confirm
Are you sure you want to perform this action?
Performing the operation "Rename" on target "PSCompression/docs/en-US123/".
[Y] Yes  [A] Yes to All  [N] No  [L] No to All  [S] Suspend  [?] Help (default is "Y"):
```

## PARAMETERS

### -NewName

Specifies the new name of the zip entry. Enter only a name, not a path and name.

> [!TIP]
> [Delay-bind scriptblock](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_script_blocks?view=powershell-7.4#using-delay-bind-script-blocks-with-parameters) is supported for this parameter. See [Example 2](#example-2-rename-all-entries-with-ext-extension-using-a-delay-bind-scriptblock).

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

The cmdlet outputs the `ZipEntryFile` and `ZipEntryDirectory` instances representing the renamed entries when this switch is used.

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

The zip entries to rename.

> [!NOTE]
>
> - This parameter takes input from pipeline, however binding by name is also possible.
> - The input are instances inheriting from `ZipEntryBase` (`ZipEntryFile` or `ZipEntryDirectory`) outputted by [`Get-ZipEntry`](Get-ZipEntry.md) and [`New-ZipEntry`](New-ZipEntry.md) cmdlets.

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

### ZipEntryBase

You can pipe instances of `ZipEntryFile` or `ZipEntryDirectory` to this cmdlet. These instances are produced by [`Get-ZipEntry`](Get-ZipEntry.md) and [`New-ZipEntry`](New-ZipEntry.md) cmdlets.

## OUTPUTS

### None

By default, this cmdlet produces no output.

### ZipEntryFile

### ZipEntryDirectory

This cmdlet outputs the renamed entries when the `-PassThru` switch is used.
