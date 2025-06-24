---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Expand-TarArchive

## SYNOPSIS

Extracts files from a tar archive, optionally compressed with gzip, bzip2, Zstandard, or lzip.

## SYNTAX

### Path

```powershell
Expand-TarArchive
    [-Path] <String[]>
    [[-Destination] <String>]
    [-Algorithm <Algorithm>]
    [-Force]
    [-PassThru]
    [<CommonParameters>]
```

### LiteralPath

```powershell
Expand-TarArchive
    -LiteralPath <String[]>
    [[-Destination] <String>]
    [-Algorithm <Algorithm>]
    [-Force]
    [-PassThru]
    [<CommonParameters>]
```

## DESCRIPTION

The `Expand-TarArchive` cmdlet extracts files and directories from a tar archive, with support for compressed tar formats using gzip (`.tar.gz`), bzip2 (`.tar.bz2`), Zstandard (`.tar.zst`), lzip (`.tar.lz`), or uncompressed tar (`.tar`). It uses libraries such as `SharpZipLib` for tar handling, `System.IO.Compression` for gzip, `SharpCompress` for lzip, and `ZstdSharp` for Zstandard. This cmdlet is the counterpart to [`Compress-TarArchive`](./Compress-TarArchive.md). By default, files are extracted to the current directory unless a `-Destination` is specified. Use `-Force` to overwrite existing files and `-PassThru` to output the extracted file and directory objects.

> [!NOTE]
> When the `-Algorithm` parameter is not specified, the cmdlet infers the compression algorithm from the file extension (e.g., `.tar.gz` for gzip, `.tar.zst` for Zstandard, `.tar` for uncompressed). See the `-Algorithm` parameter for supported extensions.

## EXAMPLES

### Example 1: Extract an uncompressed tar archive

```powershell
PS C:\> Expand-TarArchive -Path .\archive.tar

PS C:\> Get-ChildItem
    Directory: C:\

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d----          2025-06-23  7:00 PM                folder1
-a---          2025-06-23  7:00 PM           1024 file1.txt
-a---          2025-06-23  7:00 PM           2048 file2.txt
```

Extracts the contents of `archive.tar` to the current directory, creating `folder1`, `file1.txt`, and `file2.txt`.

### Example 2: Extract a gzip-compressed tar archive to a specific destination

```powershell
PS C:\> Expand-TarArchive -Path .\archive.tar.gz -Destination .\extracted

PS C:\> Get-ChildItem .\extracted
    Directory: C:\extracted

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d----          2025-06-23  7:00 PM                folder1
-a---          2025-06-23  7:00 PM           1024 file1.txt
-a---          2025-06-23  7:00 PM           2048 file2.txt
```

Extracts `archive.tar.gz` to the `extracted` directory using the gzip algorithm, creating the directory if it doesn’t exist.

### Example 3: Extract multiple Zstandard-compressed tar archives with PassThru

```powershell
PS C:\> Get-ChildItem *.tar.zst | Expand-TarArchive -PassThru

    Directory: C:\

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d----          2025-06-23  7:00 PM                folder1
-a---          2025-06-23  7:00 PM           1024 file1.txt
-a---          2025-06-23  7:00 PM           2048 file2.txt
d----          2025-06-23  7:00 PM                folder2
-a---          2025-06-23  7:00 PM           4096 file3.txt
```

Extracts all `.tar.zst` files in the current directory and outputs the extracted files and directories to the pipeline.

### Example 4: Overwrite existing files with Force

```powershell
PS C:\> Expand-TarArchive -Path .\archive.tar -Destination .\extracted -Force

PS C:\> Get-ChildItem .\extracted
    Directory: C:\extracted

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
-a---          2025-06-23  7:00 PM           1024 file1.txt
```

Extracts `archive.tar` to the `extracted` directory, overwriting any existing `file1.txt` due to the `-Force` parameter.

## PARAMETERS

### -Algorithm

Specifies the compression algorithm used for the tar archive. Accepted values and their corresponding file extensions are:

- `gz`: Gzip compression (`.gz`, `.gzip`, `.tgz`).
- `bz2`: Bzip2 compression (`.bz2`, `.bzip2`, `.tbz2`, `.tbz`)
- `zst`: Zstandard compression (`.zst`).
- `lz`: Lzip compression (`.lz`).
- `none`: Uncompressed tar archive (`.tar`).

> [!NOTE]
> If not specified, the cmdlet infers the algorithm from the file extension. If the extension is unrecognized, it defaults to `none` (uncompressed tar).

```yaml
Type: Algorithm
Parameter Sets: (All)
Aliases:
Accepted values: gz, bz2, zst, lz, none

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Destination

Specifies the path to the directory where the archive contents are extracted. If not provided, the current directory is used. If the directory does not exist, it is created.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force

Overwrites existing files in the destination directory without prompting. If not specified, the cmdlet skips files that already exist.

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

### -LiteralPath

Specifies the exact path(s) to the tar archive(s) to extract. Unlike `-Path`, `-LiteralPath` does not support wildcard characters and treats the path as a literal string. Use this parameter when the archive name contains special characters.

```yaml
Type: String[]
Parameter Sets: LiteralPath
Aliases: PSPath

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -PassThru

Outputs `System.IO.FileInfo` and `System.IO.DirectoryInfo` objects representing the extracted files and directories to the pipeline. By default, no output is produced unless an error occurs.

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

### -Path

Specifies the path(s) to the tar archive(s) to extract. Supports wildcard characters, allowing multiple archives to be processed. Paths can be provided via pipeline input.

```yaml
Type: String[]
Parameter Sets: Path
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: True
```

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

You can pipe paths to tar archives to this cmdlet via the `-Path` parameter or provide literal paths via the `-LiteralPath` parameter.

## OUTPUTS

### None

By default, this cmdlet returns no output.

### System.IO.FileSystemInfo

When the `-PassThru` parameter is used, the cmdlet outputs objects representing the extracted files and directories.

## NOTES

## RELATED LINKS

[__Compress-TarArchive__](https://github.com/santisq/PSCompression)

[__SharpZipLib__](https://github.com/icsharpcode/SharpZipLib)

[__SharpCompress__](https://github.com/adamhathcock/sharpcompress)

[__ZstdSharp__](https://github.com/oleg-st/ZstdSharp)

[__System.IO.Compression__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression?view=net-6.0)
