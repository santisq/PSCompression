---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Expand-TarEntry

## SYNOPSIS

Extracts selected tar archive entries to a destination directory while preserving their relative paths.

## SYNTAX

```powershell
Expand-TarEntry
    -InputObject <TarEntryBase[]>
    [[-Destination] <String>]
    [-Force]
    [-PassThru]
    [<CommonParameters>]
```

## DESCRIPTION

The `Expand-TarEntry` cmdlet extracts tar entries produced by [`Get-TarEntry`](./Get-TarEntry.md) to a destination directory. Extracted entries preserve their original relative paths and directory structure from the archive. It works with both uncompressed and compressed tar archives that have been processed by `Get-TarEntry`.

## EXAMPLES

### Example 1: Extract all `.txt` files from a tar archive to the current directory

```powershell
PS C:\> Get-TarEntry .\archive.tar -Include *.txt | Expand-TarEntry
```

This example extracts only the `.txt` files from `archive.tar` to the current directory, preserving their relative paths within the archive.

### Example 2: Extract all `.txt` files from a tar archive to a specific directory

```powershell
PS C:\> Get-TarEntry .\archive.tar.gz -Include *.txt | Expand-TarEntry -Destination .\extracted
```

This example extracts only the `.txt` files from a gzip-compressed tar archive to the specified `.\extracted` directory (created automatically if needed).

### Example 3: Extract all entries excluding `.txt` files from a tar archive

```powershell
PS C:\> Get-TarEntry .\archive.tar -Exclude *.txt | Expand-TarEntry
```

This example extracts everything except `.txt` files from `archive.tar` to the current directory, preserving the original structure.

### Example 4: Extract entries overwriting existing files

```powershell
PS C:\> Get-TarEntry .\archive.tar -Include *.txt | Expand-TarEntry -Force
```

This example extracts the `.txt` files and overwrites any existing files with the same name in the destination due to the `-Force` switch.

### Example 5: Extract entries and output the expanded items

```powershell
PS C:\> Get-TarEntry .\archive.tar -Exclude *.txt | Expand-TarEntry -PassThru

    Directory: C:\

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d----          2025-06-23  7:00 PM                folder1
-a---          2025-06-23  7:00 PM           2048 image.png
```

This example extracts everything except `.txt` files and uses `-PassThru` to output `FileInfo` and `DirectoryInfo` objects for the extracted items. By default, the cmdlet produces no output.

### Example 6: Extract a specific entry from a compressed tar archive

```powershell
PS C:\> $stream = Invoke-WebRequest https://example.com/archive.tar.gz
PS C:\> $stream | Get-TarEntry -Include readme.md -Algorithm gz | Expand-TarEntry -PassThru | Get-Content
```

This example extracts only the `readme.md` file from a gzip-compressed tar archive streamed from the web and immediately displays its contents.

> [!NOTE]
> When `Get-TarEntry` processes a stream, it defaults to the `gz` (gzip) algorithm. Specify `-Algorithm` on `Get-TarEntry` if the stream uses a different compression format.

## PARAMETERS

### -Destination

The destination directory where tar entries are extracted. If not specified, entries are extracted to their relative path in the current directory, creating any necessary subdirectories.

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

### -InputObject

The tar entries to extract. These are instances of `TarEntryBase` (`TarEntryFile` or `TarEntryDirectory`) output by the [`Get-TarEntry`](./Get-TarEntry.md) cmdlet.

> [!NOTE]
> This parameter accepts pipeline input from `Get-TarEntry`. Binding by name is also supported.

```yaml
Type: TarEntryBase[]
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

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### PSCompression.Abstractions.TarEntryBase[]

You can pipe instances of `TarEntryFile` or `TarEntryDirectory` from [`Get-TarEntry`](./Get-TarEntry.md) to this cmdlet.

## OUTPUTS

### None

By default, this cmdlet produces no output.

### System.IO.FileSystemInfo

When the `-PassThru` switch is used, the cmdlet outputs `FileInfo` and `DirectoryInfo` objects representing the extracted items.

## NOTES

## RELATED LINKS

[__`Get-TarEntry`__](./Get-TarEntry.md)

[__`Expand-TarArchive`__](./Expand-TarArchive.md)

[__SharpZipLib__](https://github.com/icsharpcode/SharpZipLib)

[__SharpCompress__](https://github.com/adamhathcock/sharpcompress)

[__ZstdSharp__](https://github.com/oleg-st/ZstdSharp)

[__System.IO.Compression__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression)
