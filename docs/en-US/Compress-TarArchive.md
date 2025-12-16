---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Compress-TarArchive

## SYNOPSIS

The `Compress-TarArchive` cmdlet creates a compressed tar archive file from one or more specified files or directories. It supports multiple compression algorithms and provides flexible file inclusion and exclusion options, similar to the `Compress-ZipArchive` cmdlet in this module.

## SYNTAX

### Path

```powershell
Compress-TarArchive
    [-Path] <String[]>
    [-Destination] <String>
    [-Algorithm <Algorithm>]
    [-CompressionLevel <CompressionLevel>]
    [-Force]
    [-PassThru]
    [-Exclude <String[]>]
    [<CommonParameters>]
```

### LiteralPath

```powershell
Compress-TarArchive
    -LiteralPath <String[]>
    [-Destination] <String>
    [-Algorithm <Algorithm>]
    [-CompressionLevel <CompressionLevel>]
    [-Force]
    [-PassThru]
    [-Exclude <String[]>]
    [<CommonParameters>]
```

## DESCRIPTION

The `Compress-TarArchive` cmdlet creates a tar archive, optionally compressed with algorithms like gzip, bzip2, zstd, or lz4, in a PowerShell-native environment. It simplifies file and directory archiving by integrating seamlessly with PowerShell’s object-oriented pipeline, allowing flexible file selection through cmdlets like `Get-ChildItem` or `Get-Item`. With support for selective exclusion via `-Exclude`, customizable compression levels, and the ability to overwrite existing archives, it provides a convenient alternative to traditional tar utilities for PowerShell users, while preserving directory structures and metadata.

## EXAMPLES

### Example 1: Compress all `.log` files in a directory

```powershell
Get-ChildItem C:\Logs -Recurse -Filter *.log |
    Compress-TarArchive -Destination C:\Archives\logs.tar.gz
```

This example demonstrates how to compress all `.log` files in the `C:\Logs` directory into a gzip-compressed tar archive named `logs.tar.gz` in the `C:\Archives` directory.

> [!NOTE]
> If not specified, the cmdlet uses the gzip algorithm as default.

### Example 2: Compress a folder using `Fastest` Compression Level

```powershell
Compress-TarArchive -Path . -Destination myPath.tar.gz -CompressionLevel Fastest
```

This example shows how to compress the current directory (`.`) into a gzip-compressed tar archive named `myPath.tar.gz` using the `Fastest` compression level for quicker processing.

### Example 3: Overwrite an existing tar archive

```powershell
Compress-TarArchive -Path .\Path -Destination dest.tar.gz -Force
```

This example illustrates how to create a new tar archive named `dest.tar.gz` from the path directory, overwriting any existing archive with the same name using the `-Force` parameter.

### Example 4: Exclude files and folders from source

```powershell
Compress-TarArchive -Path .\Path -Destination myPath.tar.gz -Exclude *.xyz, *\test\*
```

This example shows how to compress all items in `path` excluding all files having a `.xyz` extension, any folder named `test` and all its child items.

> [!TIP]
>
> The `-Exclude` parameter supports [wildcard patterns](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_wildcards). Exclusion patterns are tested against each item's `.FullName` property.

### Example 5: Compress a directory using bzip2 algorithm

```powershell
Compress-TarArchive -Path .\data -Destination C:\Backups\data.tar.bz2 -Algorithm bz2
```

This example demonstrates how to compress the `data` directory into a bzip2-compressed tar archive named `data.tar.bz2` in the `C:\Backups` directory using the `bz2` algorithm.

## PARAMETERS

### -Algorithm

Specifies the compression algorithm to use when creating the tar archive. Supported algorithms include `gz`, `bz2`, `zst`, `lz`, and `none` (no compression).

> [!NOTE]
> If not specified, the archive is created using the gzip algorithm (`gz`).

```yaml
Type: Algorithm
Parameter Sets: (All)
Aliases:
Accepted values: gz, bz2, zst, lz, none
Required: False
Position: Named
Default value: gz
Accept pipeline input: False
Accept wildcard characters: False
```

### -CompressionLevel

Specifies the compression level for the selected algorithm, balancing speed and file size. The default is algorithm-dependent but typically `Optimal`.

```yaml
Type: CompressionLevel
Parameter Sets: (All)
Aliases:
Accepted values: Optimal, Fastest, NoCompression, SmallestSize
Required: False
Position: Named
Default value: Optimal
Accept pipeline input: False
Accept wildcard characters: False
```

### -Destination

Specifies the path to the tar archive output file. The destination must include the file name and either an absolute or relative path.

> [!NOTE]
> If the file name lacks an extension, the `-Algorithm` parameter determines the extension is appended (e.g., `.tar.gz` for `gz`, `.tar` for `none`).

```yaml
Type: String
Parameter Sets: (All)
Aliases: DestinationPath
Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Exclude

Specifies an array of string patterns to exclude files or directories from the archive. Matching items are excluded based on their `.FullName` property. Wildcard characters are supported.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:
Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: True
```

### -Force

Overwrites the destination tar archive if it exists, creating a new one. All existing entries are lost.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:
Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -LiteralPath

Specifies the exact path or paths to the files or directories to include in the archive file. Unlike the `-Path` parameter, the value of `-LiteralPath` is used exactly as typed, with no wildcard character interpretation.

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

Outputs a `FileInfo` object representing the created tar archive. By default, the cmdlet produces no output.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:
Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Path

Specifies the path or paths to the files or directories to include in the archive file. To specify multiple paths and include files from multiple locations, use commas to separate the paths. This parameter accepts wildcard characters, allowing you to include all files in a directory or match specific patterns.

> [!TIP]
> Using wildcards with a root directory affects the archive's contents:
>
> - To create an archive that includes the root directory and all its files and subdirectories, specify the root directory without wildcards. For example: `-Path C:\Reference`
> - To create an archive that excludes the root directory but includes all its files and subdirectories, use the asterisk (`*`) wildcard. For example: `-Path C:\Reference\*`
> - To create an archive that only includes files in the root directory (excluding subdirectories), use the star-dot-star (`*.*`) wildcard. For example: `-Path C:\Reference\*.*`

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

You can pipe strings containing paths to files or directories. Output from `Get-ChildItem` or `Get-Item` can be piped to this cmdlet.

## OUTPUTS

### None

By default, this cmdlet produces no output.

### System.IO.FileInfo

When the `-PassThru` switch is used, this cmdlet outputs a `FileInfo` object representing the created tar archive.

## NOTES

This cmdlet is designed to provide a PowerShell-native way to create tar archives with flexible compression options. It integrates seamlessly with other PowerShell cmdlets for file manipulation and filtering.

## RELATED LINKS

[__`Compress-ZipArchive`__](https://github.com/santisq/PSCompression)

[__SharpZipLib__](https://github.com/icsharpcode/SharpZipLib)

[__SharpCompress__](https://github.com/adamhathcock/sharpcompress)

[__ZstdSharp__](https://github.com/oleg-st/ZstdSharp)

[__System.IO.Compression__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression)
