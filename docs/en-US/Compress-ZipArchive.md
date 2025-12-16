---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Compress-ZipArchive

## SYNOPSIS

The `Compress-ZipArchive` cmdlet creates a compressed, or zipped, archive file from one or more specified files or directories. It aims to overcome a few limitations of [`Compress-Archive`](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.archive/compress-archive) while keeping similar pipeline capabilities.

## SYNTAX

### Path

```powershell
Compress-ZipArchive
    [-Path] <String[]>
    [-Destination] <String>
    [-CompressionLevel <CompressionLevel>]
    [-Update]
    [-Force]
    [-Exclude <String[]>]
    [-PassThru]
    [<CommonParameters>]
```

### LiteralPath

```powershell
Compress-ZipArchive
    -LiteralPath <String[]>
    [-Destination] <String>
    [-CompressionLevel <CompressionLevel>]
    [-Update]
    [-Force]
    [-Exclude <String[]>]
    [-PassThru]
    [<CommonParameters>]
```

## DESCRIPTION

This cmdlet overcomes several limitations of the built-in `Compress-Archive` cmdlet:

> The `Compress-Archive` cmdlet uses the Microsoft .NET API [`System.IO.Compression.ZipArchive`](https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive) to compress files. The maximum file size is 2 GB because there's a limitation of the underlying API.

The easy workaround would be to use the [`ZipFile.CreateFromDirectory` Method](https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.zipfile.createfromdirectory#system-io-compression-zipfile-createfromdirectory(system-string-system-string)).

However, there are 3 limitations while using this method:

   1. The source __must be a directory__, a single file cannot be compressed.
   2. All files (recursively) on the source folder __will be compressed__, we can't pick / filter files to compress.
   3. It is not possible to update the entries of an existing zip archive.

This cmdlet handles compression like the `ZipFile.CreateFromDirectory` method but also allows filtering of files and folders while preserving the file and folder structure.

> [!NOTE]
> When appending to an existing zip archive using the [`-Update` parameter](#-update), a .NET limitation may cause failures for files larger than 2 GB. To handle such files, recreate the archive or use tools like 7-Zip. See [issue #19](https://github.com/santisq/PSCompression/issues/19) for details.

## EXAMPLES

### Example 1: Compress all `.ext` files from a specific folder

```powershell
Get-ChildItem .\Path -Recurse -Filter *.ext |
    Compress-ZipArchive -Destination dest.zip
```

### Example 2: Compress all `.txt` files from subfolders in the current directory

```powershell
Compress-ZipArchive .\*\*.txt -Destination dest.zip
```

### Example 3: Compress all `.ext` and `.ext2` from a specific folder

```powershell
Compress-ZipArchive .\*.ext, .\*.ext2 -Destination dest.zip
```

### Example 4: Compress a folder using `Fastest` Compression Level

```powershell
Compress-ZipArchive .\Path -Destination myPath.zip -CompressionLevel Fastest
```

### Example 5: Compressing all directories in `.\Path`

```powershell
Get-ChildItem .\Path -Recurse -Directory |
    Compress-ZipArchive -Destination dest.zip
```

### Example 6: Replacing an existing Zip Archive

```powershell
Compress-ZipArchive -Path .\Path -Destination dest.zip -Force
```

Demonstrates the use of `-Force` parameter switch. This overwrites any existing archive at the destination path.

### Example 7: Adding and updating new entries to an existing Zip Archive

```powershell
Get-ChildItem .\Path -Recurse -Directory |
    Compress-ZipArchive -Destination dest.zip -Update
```

Demonstrates the use of `-Update` parameter switch. This adds the directories to an existing archive or updates them if they already exist.

### Example 8: Exclude files and folders from source

```powershell
Compress-ZipArchive .\Path -Destination myPath.zip -Exclude *.xyz, *\test\*
```

This example shows how to compress all items in `Path` excluding all files having a `.xyz` extension and excluding
a folder named `test` and all its child items.

> [!TIP]
>
> The `-Exclude` parameter supports [wildcard patterns](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_wildcards). Exclusion patterns are tested against each item's `.FullName` property.

## PARAMETERS

### -Path

Specifies the path and file name of the output zip archive. To specify multiple paths and include files from multiple locations, use commas to separate the paths. This parameter accepts wildcard characters, allowing you to include all files in a directory or match specific patterns.

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

### -Destination

This parameter is required and specifies the path to the archive output file. The destination should include the name of the zipped file, and either the absolute or relative path to the zipped file.

> [!NOTE]
> If the file name lacks an extension, the cmdlet appends the `.zip` file name extension.

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

### -CompressionLevel

Specifies values that indicate whether a compression operation emphasizes speed or compression size. See [`CompressionLevel` Enum](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.compressionlevel) for details.

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

### -Exclude

Specifies an array of string patterns to exclude files or directories from the archive. Matching items are excluded based on their `.FullName` property. Wildcard characters are supported.

> [!NOTE]
> Patterns are tested against the object's `.FullName` property.

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

### -Update

Updates the specified archive by replacing older file versions in the archive with newer file versions that have the same names. You can also use this parameter to add files to an existing archive.

> [!NOTE]
>
> When used with `-Force`, the cmdlet adds new entries or updates existing ones instead of overwriting the entire archive.

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

### -Force

Overwrites the destination archive if it exists; otherwise, creates a new one. All existing entries are lost.

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru

Returns a `System.IO.FileInfo` object representing the created or updated zip archive.

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

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

You can pipe strings containing paths to files or directories. Output from `Get-ChildItem` or `Get-Item` can be piped to this cmdlet.

## OUTPUTS

### None

By default, this cmdlet produces no output.

### System.IO.FileInfo

When the `-PassThru` switch is used, this cmdlet outputs a `System.IO.FileInfo` object representing the created or updated archive.

## NOTES

This cmdlet was initially posted to address [this Stack Overflow question](https://stackoverflow.com/a/72611161/15339544). [Another question](https://stackoverflow.com/q/74129754/15339544) in the same site pointed out another limitation with the native cmdlet, it can't compress if another process has a handle on a file. To overcome this issue, and also to emulate explorer's behavior when compressing files used by another process, the cmdlet defaults to __[FileShare `ReadWrite, Delete`](https://learn.microsoft.com/en-us/dotnet/api/system.io.fileshare)__ when opening a [`FileStream`](https://learn.microsoft.com/en-us/dotnet/api/system.io.file.open).

## RELATED LINKS

[__System.IO.Compression__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression)

[__ZipArchive Class__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive)

[__ZipArchiveEntry Class__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchiveentry)

[__`Compress-Archive`__](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.archive/compress-archive)
