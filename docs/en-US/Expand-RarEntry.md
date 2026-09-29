---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Expand-RarEntry

## SYNOPSIS

Extracts selected RAR archive entries to a destination directory while preserving their relative paths.

## SYNTAX

```powershell
Expand-RarEntry
    -InputObject <RarEntry[]>
    [[-Destination] <String>]
    [-Password <SecureString>]
    [-Force]
    [-PassThru]
    [<CommonParameters>]
```

## DESCRIPTION

The `Expand-RarEntry` cmdlet extracts RAR entries produced by [`Get-RarEntry`](Get-RarEntry.md) to a destination directory. Extracted entries preserve their original relative paths and directory structure from the archive. It supports extracting entries from RAR archives on disk as well as from streams, including password-protected entries.

## EXAMPLES

### Example 1: Extract all `.txt` files from a RAR archive to the current directory

```powershell
PS /> Get-RarEntry .\archive.rar -Include *.txt | Expand-RarEntry
```

This example extracts only the `.txt` files from `archive.rar` to the current directory, preserving their relative paths within the archive.

### Example 2: Extract all `.txt` files from a RAR archive to a specific directory

```powershell
PS /> Get-RarEntry .\archive.rar -Include *.txt | Expand-RarEntry -Destination .\extracted
```

This example extracts only the `.txt` files from a RAR archive to the specified `.\extracted` directory (created automatically if needed).

### Example 3: Extract all entries excluding `.txt` files from a RAR archive

```powershell
PS /> Get-RarEntry .\archive.rar -Exclude *.txt | Expand-RarEntry
```

This example extracts everything except `.txt` files from `archive.rar` to the current directory, preserving the original structure.

### Example 4: Extract entries overwriting existing files

```powershell
PS /> Get-RarEntry .\archive.rar -Include *.txt | Expand-RarEntry -Force
```

This example extracts the `.txt` files and overwrites any existing files with the same name in the destination due to the `-Force` switch.

### Example 5: Extract entries and output the expanded items

```powershell
PS /> Get-RarEntry .\archive.rar -Exclude *.txt | Expand-RarEntry -PassThru

    Directory: C:\

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d----          2026-09-29  2:00 PM                folder1
-a---          2026-09-29  2:00 PM           2048 image.png
```

This example extracts everything except `.txt` files and uses `-PassThru` to output `FileInfo` and `DirectoryInfo` objects for the extracted items. By default, the cmdlet produces no output.

### Example 6: Extract a password-protected entry to the current directory

```powershell
PS /> Get-RarEntry .\archive.rar -Include secret.txt | Expand-RarEntry -Password (Read-Host -AsSecureString)
```

This example demonstrates how to expand an encrypted RAR entry using `Read-Host -AsSecureString` to provide the password.

> [!NOTE]
> If an entry is encrypted and no password is supplied, the cmdlet will prompt for one interactively.

## PARAMETERS

### -Destination

The destination directory where RAR entries are extracted. If not specified, entries are extracted to their relative path in the current directory, creating any necessary subdirectories.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force

Overwrites existing files in the destination directory. Without `-Force`, attempting to extract an entry over an existing file results in a non-terminating error.

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

The RAR entries to extract. These are instances of RarEntry output by the [`Get-RarEntry`](Get-RarEntry.md) cmdlet.

> [!NOTE]
> This parameter accepts pipeline input from `Get-RarEntry`. Binding by property name is also supported.

```yaml
Type: RarEntry[]
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -PassThru

Outputs `System.IO.FileInfo` and `System.IO.DirectoryInfo` objects for the extracted entries. By default, the cmdlet produces no output.

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

### -Password

Specifies the password as a `SecureString` to extract an encrypted RAR entry.

> [!NOTE]
> If an entry is encrypted and no password is supplied, the cmdlet will prompt for one.

```yaml
Type: SecureString
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### PSCompression.FormatHandlers.Rar.RarEntry[]

You can pipe instances of RarEntry from [`Get-RarEntry`](Get-RarEntry.md) to this cmdlet.

## OUTPUTS

### None

By default, this cmdlet produces no output.

### System.IO.FileInfo

### System.IO.DirectoryInfo

When the `-PassThru` switch is used, the cmdlet outputs `FileInfo` and `DirectoryInfo` objects representing the extracted items.

## NOTES

## RELATED LINKS

[__`Get-RarEntry`__](Get-RarEntry.md)

[__`Expand-RarArchive`__](Expand-RarArchive.md)

[__SharpCompress__](https://github.com/adamhathcock/sharpcompress)
