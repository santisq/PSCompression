---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Get-TarEntry

## SYNOPSIS

Lists tar archive entries from a specified path or input stream.

## SYNTAX

### Path (Default)

```powershell
Get-TarEntry
    [-Path] <String[]>
    [-Algorithm <Algorithm>]
    [-Type <EntryType>]
    [-Include <String[]>]
    [-Exclude <String[]>]
    [<CommonParameters>]
```

### Stream

```powershell
Get-TarEntry
    [-InputStream] <Stream>
    [-Algorithm <Algorithm>]
    [-Type <EntryType>]
    [-Include <String[]>]
    [-Exclude <String[]>]
    [<CommonParameters>]
```

### LiteralPath

```powershell
Get-TarEntry
    -LiteralPath <String[]>
    [-Algorithm <Algorithm>]
    [-Type <EntryType>]
    [-Include <String[]>]
    [-Exclude <String[]>]
    [<CommonParameters>]
```

## DESCRIPTION

The `Get-TarEntry` cmdlet lists entries in tar archives, including both uncompressed (`.tar`) and compressed formats (e.g., `.tar.gz`, `.tar.bz2`, `.tar.zst`, `.tar.lz`). It supports input from file paths or streams and outputs `TarEntryFile` or `TarEntryDirectory` objects, which can be piped to cmdlets like [`Expand-TarEntry`](./Expand-TarEntry.md) and [`Get-TarEntryContent`](./Get-TarEntryContent.md). The cmdlet uses libraries such as `SharpZipLib` for tar handling, `System.IO.Compression` for gzip, `SharpCompress` for lzip, and `ZstdSharp` for Zstandard. Use `-Include` and `-Exclude` to filter entries by name and `-Type` to filter by entry type (file or directory).

## EXAMPLES

### Example 1: List entries for a specified tar archive

```powershell
PS ..\pwsh> Get-TarEntry .\archive.tar

   Directory: /folder1/

Type                    LastWriteTime            Size Name
----                    -------------            ---- ----
Directory          6/23/2025 11:08 PM                 folder1
Archive            6/23/2025 11:08 PM         1.00 KB file1.txt
Archive            6/23/2025 11:08 PM         2.00 KB file2.txt
```

Lists all entries in `archive.tar`, including directories and files.

### Example 2: List entries from all gzip-compressed tar archives in the current directory

```powershell
PS ..\pwsh> Get-TarEntry *.tar.gz

   Directory: /folder1/

Type                    LastWriteTime            Size Name
----                    -------------            ---- ----
Directory          6/23/2025 11:08 PM                 folder1
Archive            6/23/2025 11:08 PM         1.00 KB file1.txt
Archive            6/23/2025 11:08 PM         2.00 KB file2.txt
```

> [!TIP]
> The `-Path` parameter supports wildcards.

### Example 3: List all file entries from a tar archive

```powershell
PS C:\> Get-TarEntry .\archive.tar -Type Archive

   Directory: /folder1/

Type                    LastWriteTime            Size Name
----                    -------------            ---- ----
Archive            6/23/2025 11:08 PM         1.00 KB file1.txt
Archive            6/23/2025 11:08 PM         2.00 KB file2.txt
```

Filters entries to show only files using `-Type Archive`.

### Example 4: Filter entries with Include and Exclude parameters

```powershell
PS C:\> Get-TarEntry .\archive.tbz2 -Include folder1/* -Exclude *.txt

   Directory: /folder1/

Type                    LastWriteTime             Size Name
----                    -------------             ---- ----
Directory          2025-06-23  7:00 PM                 folder1
Archive            2025-06-23  7:00 PM         3.00 KB image.png
```

Filters entries to include only those under `folder1/` but excludes `.txt` files.

> [!NOTE]
> If not specified, the cmdlet infers the compression algorithm from the file extension: `gz` for `.gz`, `.gzip`, `.tgz`; `bz2` for `.bz2`, `.bzip2`, `.tbz2`, `.tbz`; `zst` for `.zst`; `lz` for `.lz`; `none` for `.tar`. If the extension is unrecognized, it defaults to `none` (uncompressed tar).

### Example 5: List entries from an input stream

```powershell
PS C:\> $stream = Invoke-WebRequest https://example.com/archive.tar.gz
PS C:\> $stream | Get-TarEntry -Algorithm gz | Select-Object -First 3

   Directory: /docs/

Type                    LastWriteTime            Size Name
----                    -------------            ---- ----
Directory          2025-06-23  7:00 PM                 docs/
Archive            2025-06-23  7:00 PM         1.50 KB readme.md
Archive            2025-06-23  7:00 PM         2.50 KB license.txt
```

Lists the first three entries from a gzip-compressed tar archive stream, specifying `-Algorithm gz`.

> [!NOTE]
> When processing a stream, the cmdlet defaults to the `gz` (gzip) algorithm. Specify `-Algorithm` (e.g., `-Algorithm bz2` for bzip2) to match the compression type, or an error may occur if the stream is not gzip-compressed.

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

### -Exclude

Specifies an array of string patterns to match as the cmdlet lists entries. Matching entries are excluded from the output. Wildcard characters are supported.

> [!NOTE]
> Inclusion and exclusion patterns are applied to the entries’ relative paths. Exclusions are applied after inclusions.

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

### -Include

Specifies an array of string patterns to match as the cmdlet lists entries. Matching entries are included in the output. Wildcard characters are supported.

> [!NOTE]
> Inclusion and exclusion patterns are applied to the entries’ relative paths. Exclusions are applied after inclusions.

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

### -InputStream

Specifies an input stream containing a tar archive.

> [!NOTE]
>
> - Output from `Invoke-WebRequest` is automatically bound to this parameter.
> - The cmdlet defaults to the `gz` (gzip) algorithm for streams. Specify `-Algorithm` to match the compression type (e.g., `-Algorithm zst` for Zstandard) to avoid errors.

```yaml
Type: Stream
Parameter Sets: Stream
Aliases: RawContentStream

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByPropertyName, ByValue)
Accept wildcard characters: False
```

### -LiteralPath

Specifies one or more paths to tar archives. The value is used exactly as typed, with no wildcard character interpretation.

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

### -Path

Specifies one or more paths to tar archives. Wildcard characters are supported.

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

### -Type

Filters entries by type: `Archive` for files or `Directory` for directories.

```yaml
Type: EntryType
Parameter Sets: (All)
Aliases:
Accepted values: Directory, Archive

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.IO.Stream

You can pipe a stream containing a tar archive to this cmdlet, such as output from `Invoke-WebRequest`.

### System.String[]

You can pipe strings containing paths to tar archives, such as output from `Get-ChildItem` or `Get-Item`.

## OUTPUTS

### PSCompression.TarEntryDirectory

### PSCompression.TarEntryFile

Outputs objects representing directories or files in the tar archive.

## NOTES

## RELATED LINKS

[__Expand-TarEntry__](https://github.com/santisq/PSCompression)

[__Expand-TarArchive__](https://github.com/santisq/PSCompression)

[__Get-TarEntryContent__](https://github.com/santisq/PSCompression)

[__SharpZipLib__](https://github.com/icsharpcode/SharpZipLib)

[__SharpCompress__](https://github.com/adamhathcock/sharpcompress)

[__ZstdSharp__](https://github.com/oleg-st/ZstdSharp)

[__System.IO.Compression__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression?view=net-6.0)
