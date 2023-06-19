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

### Path (Default)

```powershell
Compress-ZipArchive [-Path] <String[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-PassThru] [<CommonParameters>]
```

### PathWithForce

```powershell
Compress-ZipArchive [-Path] <String[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-Force] [-PassThru] [<CommonParameters>]
```

### PathWithUpdate

```powershell
Compress-ZipArchive [-Path] <String[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-Update] [-PassThru] [<CommonParameters>]
```

### LiteralPath

```powershell
Compress-ZipArchive -LiteralPath <String[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-PassThru] [<CommonParameters>]
```

### LiteralPathWithForce

```powershell
Compress-ZipArchive -LiteralPath <String[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-Force] [-PassThru] [<CommonParameters>]
```

### LiteralPathWithUpdate

```powershell
Compress-ZipArchive -LiteralPath <String[]> [-DestinationPath] <String> [-CompressionLevel <CompressionLevel>]
 [-Update] [-PassThru] [<CommonParameters>]
```

## DESCRIPTION

PowerShell cmdlet that overcomes the limitation that the built-in cmdlet `Compress-Archive` has:

> The `Compress-Archive` cmdlet uses the Microsoft .NET API [`System.IO.Compression.ZipArchive`](https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive?view=net-6.0) to compress files. The maximum file size is 2 GB because there's a limitation of the underlying API.

The easy workaround would be to use the [`ZipFile.CreateFromDirectory` Method](https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.zipfile.createfromdirectory?view=net-6.0#system-io-compression-zipfile-createfromdirectory(system-string-system-string)). However, there are 3 limitations while using this static method:

   1. The source __must be a directory__, a single file cannot be compressed.
   2. All files (recursively) on the source folder __will be compressed__, we can't pick / filter files to compress.
   3. It's not possible to __Update__ the entries of an existing Zip Archive.

This function should be able to handle compression same as `ZipFile.CreateFromDirectory` Method but also allow filtering files and folders to compress while keeping the __file / folder structure untouched__.

## EXAMPLES

### Example 1: Compress all `.ext` files from a specific folder

```powershell
Get-ChildItem .\path -Recurse -Filter *.ext |
    Compress-ZipArchive -DestinationPath dest.zip
```

### Example 2: Compress all `.txt` files contained in all folders in the Current Directory

```powershell
Compress-ZipArchive .\*\*.txt -DestinationPath dest.zip
```

### Example 3: Compress all `.ext` and `.ext2` from a specific folder

```powershell
Compress-ZipArchive .\*.ext, .\*.ext2 -DestinationPath dest.zip
```

### Example 4: Compress a folder using `Fastest` Compression Level

```powershell
Compress-ZipArchive .\path -Destination myPath.zip -CompressionLevel Fastest
```

### Example 5: Compressing all directories in `.\Path`

```powershell
Get-ChildItem .\path -Recurse -Directory |
    Compress-ZipArchive -DestinationPath dest.zip
```

### Example 6: Replacing an existing Zip Archive

Demonstrates the use of `-Force` parameter switch.

```powershell
Compress-ZipArchive -Path .\path -DestinationPath dest.zip -Force
```

### Example 7: Adding and updating new entries to an existing Zip Archive

Demonstrates the use of `-Update` parameter switch.

```powershell
Get-ChildItem .\path -Recurse -Directory |
    Compress-ZipArchive -DestinationPath dest.zip -Update
```

## PARAMETERS

### -Path

Specifies the path or paths to the files to add to the archive zipped file.
To specify multiple paths, and include files in multiple locations, use commas to separate the paths.
This Parameter accepts wildcard characters.
Wildcard characters allow you to add all files in a directory to your archive file.

```yaml
Type: String[]
Parameter Sets: Path, PathWithForce, PathWithUpdate
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -LiteralPath

Specifies the path or paths to the files that you want to add to the archive zipped file.
Unlike the Path `-Parameter`, the value of `-LiteralPath` is used exactly as it's typed.
No characters are interpreted as wildcards

```yaml
Type: String[]
Parameter Sets: LiteralPath, LiteralPathWithForce, LiteralPathWithUpdate
Aliases: PSPath

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -DestinationPath

The destination path to the Zip file.
If the file name in DestinationPath doesn't have a `.zip` file name extension, the function appends the `.zip` file name extension.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 2
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

### -Update

Updates Zip entries and adds new entries to an existing Zip file.

```yaml
Type: SwitchParameter
Parameter Sets: PathWithUpdate, LiteralPathWithUpdate
Aliases:

Required: True
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force

Replaces an existing Zip file with a new one.
All Zip contents will be lost.

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

Outputs the object representing the compressed file.
The function produces no output by default.

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

You can pipe a string that contains a path to one or more files.

## OUTPUTS

### None

By default, this cmdlet produces no output.

### FileInfo

When the `-PassThru` switch is used this cmdlet outputs the `FileInfo` instance representing the compressed file.

## NOTES

This function was initially posted to address [this Stack Overflow question](https://stackoverflow.com/a/72611161/15339544). [Another question](https://stackoverflow.com/q/74129754/15339544) in the same site pointed out another limitation with the native cmdlet, it can't compress if another process has a handle on a file. To overcome this issue, and also to emulate explorer's behavior when compressing files used by another process, the function posted below will default to __[`[FileShare] 'ReadWrite, Delete'`](https://learn.microsoft.com/en-us/dotnet/api/system.io.fileshare?view=net-6.0)__ when opening a [`FileStream`](https://learn.microsoft.com/en-us/dotnet/api/system.io.file.open?view=net-7.0).
