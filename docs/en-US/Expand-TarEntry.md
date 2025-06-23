---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Expand-TarEntry

## SYNOPSIS

Expands tar archive entries to a destination directory.

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

The `Expand-TarEntry` cmdlet extracts tar entries output by the [`Get-TarEntry`](./Get-TarEntry.md) cmdlet to a destination directory. Expanded entries maintain their original folder structure based on their relative path within the tar archive. This cmdlet supports both uncompressed (`.tar`) and compressed tar archives (e.g., `.tar.gz`, `.tar.bz2`) processed by `Get-TarEntry`.

## EXAMPLES

### Example 1: Extract all `.txt` files from a tar archive to the current directory

```powershell
PS C:\> Get-TarEntry .\archive.tar -Include *.txt | Expand-TarEntry
```

Extracts all `.txt` files from `archive.tar` to the current directory, preserving their relative paths.

### Example 2: Extract all `.txt` files from a tar archive to a specific directory

```powershell
PS C:\> Get-TarEntry .\archive.tar.gz -Include *.txt | Expand-TarEntry -Destination .\extracted
```

Extracts all `.txt` files from `archive.tar.gz` to the `extracted` directory, creating the directory if it doesn’t exist.

### Example 3: Extract all entries excluding `.txt` files from a tar archive

```powershell
PS C:\> Get-TarEntry .\archive.tar -Exclude *.txt | Expand-TarEntry
```

Extracts all entries except `.txt` files from `archive.tar` to the current directory.

### Example 4: Extract entries overwriting existing files

```powershell
PS C:\> Get-TarEntry .\archive.tar -Include *.txt | Expand-TarEntry -Force
```

Demonstrates how the `-Force` switch overwrites existing files in the destination directory.

### Example 5: Extract entries and output the expanded items

```powershell
PS C:\> Get-TarEntry .\archive.tar -Exclude *.txt | Expand-TarEntry -PassThru

    Directory: C:\

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d----          2025-06-23  7:00 PM                folder1
-a---          2025-06-23  7:00 PM           2048 image.png
```

By default, this cmdlet produces no output. When `-PassThru` is used, it outputs `FileInfo` and `DirectoryInfo` objects representing the extracted entries.

### Example 6: Extract a specific entry from a compressed tar archive

```powershell
PS C:\> $stream = Invoke-WebRequest https://example.com/archive.tar.gz
PS C:\> $file = $stream | Get-TarEntry -Include readme.md -Algorithm gz | Expand-TarEntry -PassThru
PS C:\> Get-Content $file.FullName

# My Project
This is the README file for my project.
```

Extracts the `readme.md` file from a gzip-compressed tar archive retrieved via a web request and displays its contents.

> [!NOTE]
> When `Get-TarEntry` processes a stream, it defaults to the `gz` (gzip) algorithm. Specify the `-Algorithm` parameter (e.g., `-Algorithm bz2` for bzip2) to match the compression type of the tar archive, or an error may occur if the stream is not gzip-compressed.

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

Overwrites existing files in the destination directory when this switch is used.

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

Outputs `FileInfo` and `DirectoryInfo` objects representing the extracted entries when this switch is used. By default, the cmdlet produces no output.

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

When the `-PassThru` switch is used, the cmdlet outputs objects representing the extracted files and directories.

## NOTES

## RELATED LINKS

[__Get-TarEntry__](https://github.com/santisq/PSCompression)

[__Expand-TarArchive__](https://github.com/santisq/PSCompression)

[__SharpZipLib__](https://github.com/icsharpcode/SharpZipLib)

[__SharpCompress__](https://github.com/adamhathcock/sharpcompress)

[__ZstdSharp__](https://github.com/oleg-st/ZstdSharp)

[__System.IO.Compression__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression)
