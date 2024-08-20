---
external help file: PSCompression-help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Compress-ZipArchive

## SYNOPSIS

The `Compress-ZipArchive` cmdlet creates a compressed, or zipped, archive file from one or more specified files or directories. It aims to overcome a few limitations of [`Compress-Archive`](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.archive/compress-archive?view=powershell-7.2) while keeping similar pipeline capabilities.

## SYNTAX

### Path

```powershell
Compress-ZipArchive
    [-Path] <String[]>
    -Destination <String>
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
    -Destination <String>
    [-CompressionLevel <CompressionLevel>]
    [-Update]
    [-Force]
    [-Exclude <String[]>]
    [-PassThru]
    [<CommonParameters>]
```

## DESCRIPTION

PowerShell cmdlet that overcomes the limitation that the built-in cmdlet `Compress-Archive` has:

> The `Compress-Archive` cmdlet uses the Microsoft .NET API [`System.IO.Compression.ZipArchive`](https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive?view=net-6.0) to compress files. The maximum file size is 2 GB because there's a limitation of the underlying API.

The easy workaround would be to use the [`ZipFile.CreateFromDirectory` Method](https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.zipfile.createfromdirectory?view=net-6.0#system-io-compression-zipfile-createfromdirectory(system-string-system-string)).

However, there are 3 limitations while using this method:

   1. The source __must be a directory__, a single file cannot be compressed.
   2. All files (recursively) on the source folder __will be compressed__, we can't pick / filter files to compress.
   3. It's not possible to __Update__ the entries of an existing Zip Archive.

This cmdlet should be able to handle compression same as `ZipFile.CreateFromDirectory` Method but also allow filtering files and folders to compress while keeping the __file / folder structure untouched__.

## EXAMPLES

### Example 1: Compress all `.ext` files from a specific folder

```powershell
Get-ChildItem .\path -Recurse -Filter *.ext |
    Compress-ZipArchive -Destination dest.zip
```

### Example 2: Compress all `.txt` files contained in all folders in the Current Directory

```powershell
Compress-ZipArchive .\*\*.txt -Destination dest.zip
```

### Example 3: Compress all `.ext` and `.ext2` from a specific folder

```powershell
Compress-ZipArchive .\*.ext, .\*.ext2 -Destination dest.zip
```

### Example 4: Compress a folder using `Fastest` Compression Level

```powershell
Compress-ZipArchive .\path -Destination myPath.zip -CompressionLevel Fastest
```

### Example 5: Compressing all directories in `.\Path`

```powershell
Get-ChildItem .\path -Recurse -Directory |
    Compress-ZipArchive -Destination dest.zip
```

### Example 6: Replacing an existing Zip Archive

Demonstrates the use of `-Force` parameter switch.

```powershell
Compress-ZipArchive -Path .\path -Destination dest.zip -Force
```

### Example 7: Adding and updating new entries to an existing Zip Archive

Demonstrates the use of `-Update` parameter switch.

```powershell
Get-ChildItem .\path -Recurse -Directory |
    Compress-ZipArchive -Destination dest.zip -Update
```

### Example 8: Exclude files and folders from source

```powershell
Compress-ZipArchive .\path -Destination myPath.zip -Exclude *.xyz, *\test\*
```

This example shows how to compress all items in `path` excluding all files having a `.xyz` extension and excluding
a folder with name `test` and all its child items.

> [!TIP]
>
> The `-Exclude` parameter supports [wildcard patterns](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_wildcards?view=powershell-7.4&viewFallbackFrom=powershell-7.3),
exclusion patterns are tested against the items `.FullName` property.

## PARAMETERS

### -Path

Specifies the path or paths to the files that you want to add to the archive zipped file. To specify multiple paths, and include files in multiple locations, use commas to separate the paths.

This parameter accepts wildcard characters. Wildcard characters allow you to add all files in a directory to your archive file.

> [!TIP]
> Using wildcards with a root directory affects the archive's contents:
>
> - To create an archive that includes the root directory, and all its files and subdirectories, specify the root directory in the Path without wildcards. For example: `-Path C:\Reference`
> - To create an archive that excludes the root directory, but zips all its files and subdirectories, use the asterisk (`*`) wildcard. For example: `-Path C:\Reference\*`
> - To create an archive that only zips the files in the root directory, use the star-dot-star (`*.*`) wildcard. Subdirectories of the root aren't included in the archive. For example: `-Path C:\Reference\*.*`

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

Specifies the path or paths to the files that you want to add to the archive zipped file.
Unlike the Path `-Parameter`, the value of `-LiteralPath` is used exactly as it's typed.
No characters are interpreted as wildcards

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
> If the file name does not have an extension, the cmdlet appends the `.zip` file name extension.

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

Specifies an array of one or more string patterns to be matched as the cmdlet gets child items.
Any matching item is excluded from the created zip archive.
Wildcard characters are accepted.

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
> If `-Force` and `-Update` are used together this cmdlet will add or update entries.

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

Overwrites the destination archive if exists otherwise it creates a new one. All Zip entries are lost.

> [!NOTE]
> If `-Force` and `-Update` are used together this cmdlet will add or update entries.

```yaml
Type: SwitchParameter
Parameter Sets: PathWithForce, LiteralPathWithForce
Aliases:

Required: True
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru

Outputs the object representing the compressed file. The cmdlet produces no output by default.

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

### String

You can pipe a string that contains a path to one or more files. Output from `Get-ChildItem` or `Get-Item` can be piped to this cmdlet.

## OUTPUTS

### None

By default, this cmdlet produces no output.

### FileInfo

When the `-PassThru` switch is used this cmdlet outputs the `FileInfo` instance representing the compressed file.

## NOTES

This cmdlet was initially posted to address [this Stack Overflow question](https://stackoverflow.com/a/72611161/15339544). [Another question](https://stackoverflow.com/q/74129754/15339544) in the same site pointed out another limitation with the native cmdlet, it can't compress if another process has a handle on a file. To overcome this issue, and also to emulate explorer's behavior when compressing files used by another process, the cmdlet defaults to __[`FileShare 'ReadWrite, Delete'`](https://learn.microsoft.com/en-us/dotnet/api/system.io.fileshare?view=net-6.0)__ when opening a [`FileStream`](https://learn.microsoft.com/en-us/dotnet/api/system.io.file.open?view=net-7.0).
