---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Get-RarEntry

## SYNOPSIS

Lists entries from RAR archives, supporting file paths and input streams.

## SYNTAX

### Path (Default)

```powershell
Get-RarEntry
    [-Path] <String[]>
    [-Type <EntryType>]
    [-Include <String[]>]
    [-Exclude <String[]>]
    [<CommonParameters>]
```

### LiteralPath

```powershell
Get-RarEntry
    -LiteralPath <String[]>
    [-Type <EntryType>]
    [-Include <String[]>]
    [-Exclude <String[]>]
    [<CommonParameters>]
```

### Stream

```powershell
Get-RarEntry
    [-InputStream] <Stream>
    [-Type <EntryType>]
    [-Include <String[]>]
    [-Exclude <String[]>]
    [<CommonParameters>]
```

## DESCRIPTION

The `Get-RarEntry` cmdlet lists entries in RAR archives. It accepts input from file paths and streams, and outputs `RarEntry` objects that can be piped to cmdlets like [`Expand-RarEntry`](Expand-RarEntry.md) and [`Get-RarEntryContent`](Get-RarEntryContent.md). The cmdlet uses `SharpCompress` for RAR archive processing.

## EXAMPLES

### Example 1: List entries for a specified RAR archive

```powershell
PS /> Get-RarEntry .\archive.rar

   Directory: /folder1/

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Directory          2026-09-29  2:00 PM                                folder1
Archive            2026-09-29  2:00 PM       450.00  B        1.00 KB file1.txt
Archive            2026-09-29  2:00 PM       900.00  B        2.00 KB file2.txt
```

This example lists all entries in `archive.rar`.

### Example 2: List entries from all RAR archives in the current directory

```powershell
PS /> Get-RarEntry *.rar
```

This example lists entries from all RAR archives in the current directory using wildcard matching.

### Example 3: List all file entries from a RAR archive

```powershell
PS /> Get-RarEntry .\archive.rar -Type Archive

   Directory: /folder1/

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Archive            2026-09-29  2:00 PM       450.00  B        1.00 KB file1.txt
Archive            2026-09-29  2:00 PM       900.00  B        2.00 KB file2.txt
```

This example lists only file entries (excluding directories) from `archive.rar` using `-Type Archive`.

### Example 4: Filter entries with Include and Exclude parameters

```powershell
PS /> Get-RarEntry .\archive.rar -Include folder1/* -Exclude *.txt

   Directory: /folder1/

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Directory          2026-09-29  2:00 PM                                folder1
Archive            2026-09-29  2:00 PM         1.20 KB        3.00 KB image.png
```

This example lists entries under `folder1/` while excluding any `.txt` files.

> [!NOTE]
> Inclusion and exclusion patterns are applied to the entries' relative paths. Exclusions are applied after inclusions.

### Example 5: List entries from an input stream

```powershell
PS /> $stream = Invoke-WebRequest https://example.com/archive.rar
PS /> $stream | Get-RarEntry | Select-Object -First 3

   Directory: /docs/

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Directory          2026-09-29  2:00 PM                                docs
Archive            2026-09-29  2:00 PM       600.00  B        1.50 KB readme.md
Archive            2026-09-29  2:00 PM         1.00 KB        2.50 KB license.txt
```

This example lists the first three entries from a RAR archive stream retrieved via a web request.

## PARAMETERS

### -Exclude

Specifies an array of string patterns to match as the cmdlet lists entries. Matching entries are excluded from the output. Wildcard characters are supported.

> [!NOTE]
> Inclusion and exclusion patterns are applied to the entries' relative paths. Exclusions are applied after inclusions.

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
> Inclusion and exclusion patterns are applied to the entries' relative paths. Exclusions are applied after inclusions.

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

Specifies an input stream containing a RAR archive.

> [!TIP]
> Output from `Invoke-WebRequest` is automatically bound to this parameter.

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

Specifies one or more paths to RAR archives. The value is used exactly as typed, with no wildcard character interpretation.

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

Specifies one or more paths to RAR archives. Wildcard characters are supported.

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

Filters output to include only files (`Archive`) or only directories (`Directory`).

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

You can pipe a stream containing a RAR archive to this cmdlet, such as output from `Invoke-WebRequest`.

### System.String[]

You can pipe strings containing paths to RAR archives, such as output from `Get-ChildItem` or `Get-Item`.

## OUTPUTS

### PSCompression.FormatHandlers.Rar.RarEntry

Outputs `RarEntry` objects representing files or directories in the RAR archive.

## NOTES

## RELATED LINKS

[__`Expand-RarEntry`__](Expand-RarEntry.md)

[__`Expand-RarArchive`__](Expand-RarArchive.md)

[__SharpCompress__](https://github.com/adamhathcock/sharpcompress)
