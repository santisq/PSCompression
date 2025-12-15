---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Expand-ZipEntry

## SYNOPSIS

Extracts selected zip archive entries to a destination directory while preserving their relative paths.

## SYNTAX

```powershell
Expand-ZipEntry
    -InputObject <ZipEntryBase[]>
    [-Destination <String>]
    [-Force]
    [-PassThru]
    [-Password <SecureString>]
    [<CommonParameters>]
```

## DESCRIPTION

The `Expand-ZipEntry` cmdlet extracts zip entries produced by [`Get-ZipEntry`](./Get-ZipEntry.md) (or [`New-ZipEntry`](./New-ZipEntry.md)) to a destination directory. Extracted entries preserve their original relative paths and directory structure from the archive. It also supports extracting password-protected entries.

## EXAMPLES

### Example 1: Extract all `.txt` files from a Zip Archive to the current directory

```powershell
PS ..\pwsh> Get-ZipEntry path\to\myZip.zip -Include *.txt | Expand-ZipEntry
```

This example extracts only the `.txt` files from a zip archive to the current directory, preserving their relative paths within the archive.

### Example 2: Extract all `.txt` files from a Zip Archive to the a desired directory

```powershell
PS ..\pwsh> Get-ZipEntry path\to\myZip.zip -Include *.txt | Expand-ZipEntry -Destination path\to\myfolder
```

This example extracts only the `.txt` files from a zip archive to the specified destination directory (created automatically if needed).

### Example 3: Extract all entries excluding `.txt` files to the current directory

```powershell
PS ..\pwsh> Get-ZipEntry path\to\myZip.zip -Exclude *.txt | Expand-ZipEntry
```

This example extracts everything except `.txt` files from a zip archive to the current directory, preserving the original structure.

### Example 4: Extract all entries excluding `.txt` files to the current directory overwritting existing files

```powershell
PS ..\pwsh> Get-ZipEntry path\to\myZip.zip -Exclude *.txt | Expand-ZipEntry -Force
```

This example extracts everything except `.txt` files and overwrites any existing files with the same name due to the `-Force` switch.

### Example 5: Extract all entries excluding `.txt` files to the current directory outputting the expanded entries

```powershell
PS ..\pwsh> Get-ZipEntry path\to\myZip.zip -Exclude *.txt | Expand-ZipEntry -PassThru
```

This example extracts everything except `.txt` files and uses `-PassThru` to output `FileInfo` and `DirectoryInfo` objects for the extracted items. By default, the cmdlet produces no output.

### Example 6: Extract an entry from input Stream

```powershell
PS ..\pwsh> $package = Invoke-WebRequest https://www.powershellgallery.com/api/v2/package/PSCompression
PS ..\pwsh> $file = $package | Get-ZipEntry -Include *.psd1 | Expand-ZipEntry -PassThru
PS ..\pwsh> Get-Content $file.FullName -Raw | Invoke-Expression

Name                           Value
----                           -----
PowerShellVersion              5.1
Description                    Zip and GZip utilities for PowerShell!
RootModule                     bin/netstandard2.0/PSCompression.dll
FormatsToProcess               {PSCompression.Format.ps1xml}
VariablesToExport              {}
PrivateData                    {[PSData, System.Collections.Hashtable]}
CmdletsToExport                {Get-ZipEntry, Get-ZipEntryContent, Set-ZipEntryContent, Remove-ZipEntry…}
ModuleVersion                  2.0.10
Author                         Santiago Squarzon
CompanyName                    Unknown
GUID                           c63aa90e-ae64-4ae1-b1c8-456e0d13967e
FunctionsToExport              {}
RequiredAssemblies             {System.IO.Compression, System.IO.Compression.FileSystem}
Copyright                      (c) Santiago Squarzon. All rights reserved.
AliasesToExport                {gziptofile, gzipfromfile, gziptostring, gzipfromstring…}
```

This example downloads a NuGet package (which is a zip archive) from PowerShell Gallery, extracts the manifest (`.psd1`) file, and immediately evaluates it to display module metadata.

### Example 7: Expand a password protected entry to the current directory

```powershell
PS ..\pwsh> Get-ZipEntry .\myZip.zip -Include myEncryptedEntry.txt | Expand-ZipEntry -Password (Read-Host -AsSecureString)
```

This example demonstrates how to expand an encrypted entry using `Read-Host -AsSecureString` to provide the password.

> [!TIP]
> If an entry is encrypted and no password is supplied, the cmdlet will prompt for one.

## PARAMETERS

### -Destination

Specifies the root directory where zip entries are extracted.

> [!NOTE]
> This parameter is optional. When not used, entries are extracted relative to the current directory, preserving their paths from the archive and creating subdirectories as needed.

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

Overwrites existing files in the destination directory. Without `-Force`, existing files are skipped.

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

Specifies the password as a `SecureString` to extract the encrypted zip entry.

> [!TIP]
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

### -InputObject

The zip entries to expand.

> [!NOTE]
>
> - This parameter accepts pipeline input (by value). Binding by property name is also supported.
> - The input are instances inheriting from `ZipEntryBase` (`ZipEntryFile` or `ZipEntryDirectory`) produced by [`Get-ZipEntry`](./Get-ZipEntry.md) and [`New-ZipEntry`](./New-ZipEntry.md) cmdlets.

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

### PSCompression.Abstractions.ZipEntryBase[]

You can pipe instances of `ZipEntryFile` or `ZipEntryDirectory` to this cmdlet. These instances are produced by [`Get-ZipEntry`](./Get-ZipEntry.md) and [`New-ZipEntry`](./New-ZipEntry.md).

## OUTPUTS

### None

By default, this cmdlet produces no output.

### System.IO.FileSystemInfo

When the `-PassThru` switch is used, the cmdlet outputs `FileInfo` and `DirectoryInfo` objects representing the extracted items.

## NOTES

## RELATED LINKS

[__`Get-ZipEntry`__](./Get-ZipEntry.md)

[__SharpZipLib__](https://github.com/icsharpcode/SharpZipLib)
