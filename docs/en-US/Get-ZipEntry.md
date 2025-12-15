---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Get-ZipEntry

## SYNOPSIS

Lists entries from zip archives, supporting file paths and input streams.

## SYNTAX

### Path (Default)

```powershell
Get-ZipEntry
   -Path <String[]>
   [-Type <String>]
   [-Include <String[]>]
   [-Exclude <String[]>]
   [<CommonParameters>]
```

### LiteralPath

```powershell
Get-ZipEntry
   -LiteralPath <String[]> 
   [-Type <String>]
   [-Include <String[]>]
   [-Exclude <String[]>]
   [<CommonParameters>]
```

### Stream

```powershell
Get-ZipEntry
   -InputStream <Stream>
   [-Type <EntryType>]
   [-Include <String[]>]
   [-Exclude <String[]>]
   [<CommonParameters>]
```

## DESCRIPTION

The `Get-ZipEntry` cmdlet lists entries in zip archives. It supports input from file paths and streams, and outputs `ZipEntryFile` or `ZipEntryDirectory` objects that can be piped to other `*-ZipEntry` cmdlets such as [`Expand-ZipEntry`](./Expand-ZipEntry.md) or [`Get-ZipEntryContent`](./Get-ZipEntryContent.md).

## EXAMPLES

### Example 1: List entries for a specified file path

```powershell
PS ..\pwsh> Get-ZipEntry path\to\myZip.zip
```

This example lists all entries in the specified zip archive.

### Example 2: List entries from all files with `.zip` extension in the current directory

```powershell
PS ..\pwsh> Get-ZipEntry *.zip
```

This example lists entries from all `.zip` files in the current directory. The `-Path` parameter supports wildcards.

### Example 3: List all `Archive` entries from a Zip file

```powershell
PS ..\pwsh> Get-ZipEntry path\to\myZip.zip -Type Archive
```

> [!TIP]
> The `-Type` parameter supports filtering by `Archive` or `Directory`.

### Example 4: Filtering entries with `-Include` and `-Exclude` parameters

```powershell
PS ..\pwsh> Get-ZipEntry .\PSCompression.zip -Include PSCompression/docs/en-us*

   Directory: /PSCompression/docs/en-US/

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Directory          2/22/2024  1:19 PM         0.00  B                 en-US
Archive            2/22/2024  1:19 PM         2.08 KB         6.98 KB Compress-GzipArchive.md
Archive            2/22/2024  1:19 PM         2.74 KB         8.60 KB Compress-ZipArchive.md
Archive            2/22/2024  1:19 PM         1.08 KB         2.67 KB ConvertFrom-GzipString.md
Archive            2/22/2024  1:19 PM         1.67 KB         4.63 KB ConvertTo-GzipString.md
Archive            2/22/2024  1:19 PM         1.74 KB         6.28 KB Expand-GzipArchive.md
Archive            2/22/2024  1:19 PM         1.23 KB         4.07 KB Expand-ZipEntry.md
Archive            2/22/2024  1:19 PM         1.53 KB         6.38 KB Get-ZipEntry.md
Archive            2/22/2024  1:19 PM         1.67 KB         5.06 KB Get-ZipEntryContent.md
Archive            2/22/2024  1:19 PM         2.20 KB         7.35 KB New-ZipEntry.md
Archive            2/22/2024  1:19 PM       961.00  B         2.62 KB PSCompression.md
Archive            2/22/2024  1:19 PM         1.14 KB         2.95 KB Remove-ZipEntry.md
Archive            2/22/2024  1:19 PM       741.00  B         2.16 KB Rename-ZipEntry.md
Archive            2/22/2024  1:19 PM         1.55 KB         5.35 KB Set-ZipEntryContent.md

PS ..\pwsh> Get-ZipEntry .\PSCompression.zip -Include PSCompression/docs/en-us* -Exclude *en-US/Compress*, *en-US/Remove*

   Directory: /PSCompression/docs/en-US/

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Directory          2/22/2024  1:19 PM         0.00  B                 en-US
Archive            2/22/2024  1:19 PM         1.08 KB         2.67 KB ConvertFrom-GzipString.md
Archive            2/22/2024  1:19 PM         1.67 KB         4.63 KB ConvertTo-GzipString.md
Archive            2/22/2024  1:19 PM         1.74 KB         6.28 KB Expand-GzipArchive.md
Archive            2/22/2024  1:19 PM         1.23 KB         4.07 KB Expand-ZipEntry.md
Archive            2/22/2024  1:19 PM         1.53 KB         6.38 KB Get-ZipEntry.md
Archive            2/22/2024  1:19 PM         1.67 KB         5.06 KB Get-ZipEntryContent.md
Archive            2/22/2024  1:19 PM         2.20 KB         7.35 KB New-ZipEntry.md
Archive            2/22/2024  1:19 PM       961.00  B         2.62 KB PSCompression.md
Archive            2/22/2024  1:19 PM       741.00  B         2.16 KB Rename-ZipEntry.md
Archive            2/22/2024  1:19 PM         1.55 KB         5.35 KB Set-ZipEntryContent.md
```

> [!NOTE]
>
> - Inclusion and Exclusion patterns are applied to the entries relative path.
> - Exclusions are applied after the inclusions.

### Example 5: List entries from an input Stream

```powershell
PS ..\pwsh> $package = Invoke-WebRequest https://www.powershellgallery.com/api/v2/package/PSCompression
PS ..\pwsh> $package | Get-ZipEntry | Select-Object -First 5

   Directory: /

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Archive            11/6/2024 10:29 PM       227.00  B       785.00  B [Content_Types].xml
Archive            11/6/2024 10:27 PM       516.00  B         2.50 KB PSCompression.Format.ps1xml
Archive            11/6/2024 10:29 PM       598.00  B         1.58 KB PSCompression.nuspec
Archive            11/6/2024 10:27 PM         1.66 KB         5.45 KB PSCompression.psd1

   Directory: /_rels/

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Archive            11/6/2024 10:29 PM       276.00  B       507.00  B .rels

   Directory: /bin/netstandard2.0/

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Archive            11/6/2024 10:28 PM       996.00  B         3.12 KB PSCompression.deps.json
Archive            11/6/2024 10:28 PM        28.73 KB        66.00 KB PSCompression.dll
Archive            11/6/2024 10:28 PM        14.75 KB        29.39 KB PSCompression.pdb

   Directory: /en-US/

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Archive            11/6/2024 10:28 PM         8.33 KB       106.86 KB PSCompression-help.xml
Archive            11/6/2024 10:28 PM         9.19 KB       103.84 KB PSCompression.dll-Help.xml

   Directory: /package/services/metadata/core-properties/

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Archive            11/6/2024 10:29 PM       635.00  B         1.55 KB 3212d87de09c4241a06e0166a08c3b13.psmdcp
```

This example downloads a NuGet package (a zip archive) from PowerShell Gallery and lists the first five entries from the streamed content.

## PARAMETERS

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

### -LiteralPath

Specifies a path to one or more zip archives. Note that the value is used exactly as it's typed. No characters are interpreted as wildcards.

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

Specifies a path to one or more zip archives. Wildcards are accepted.

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

### -InputStream

Specifies an input stream containing a zip archive.

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

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

You can pipe a string that contains a paths to this cmdlet.  Output from `Get-ChildItem` or `Get-Item` can be piped to this cmdlet.

### System.IO.Stream

You can pipe a stream containing a zip archive (e.g., output from `Invoke-WebRequest`) to this cmdlet via the `-InputStream` parameter.

## OUTPUTS

### PSCompression.ZipEntryDirectory

### PSCompression.ZipEntryFile

## NOTES

## RELATED LINKS

[__SharpZipLib__](https://github.com/icsharpcode/SharpZipLib)

[__ZipFile__](https://icsharpcode.github.io/SharpZipLib/help/api/ICSharpCode.SharpZipLib.Zip.ZipFile.html)

[__ZipEntry__](https://icsharpcode.github.io/SharpZipLib/api/ICSharpCode.SharpZipLib.Zip.ZipEntry.html)
