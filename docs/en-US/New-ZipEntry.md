---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# New-ZipEntry

## SYNOPSIS

Creates new entries in a zip archive from strings or existing files.

## SYNTAX

### Value (Default)

```powershell
New-ZipEntry
   -Destination <String>
   -EntryPath <String[]>
   [-Value <String[]>]
   [-CompressionLevel <CompressionLevel>]
   [-Encoding <Encoding>]
   [-Force]
   [<CommonParameters>]
```

### File

```powershell
New-ZipEntry
   -Destination <String>
   -SourcePath <String>
   [-EntryPath <String[]>]
   [-CompressionLevel <CompressionLevel>]
   [-Force]
   [<CommonParameters>]
```

## DESCRIPTION

The `New-ZipEntry` cmdlet creates new entries in an existing or new zip archive. Entries can be created from string input (`-Value`) or by copying files from the filesystem (`-SourcePath`). The type of entry (file or directory) is determined by the provided `-EntryPath`: paths ending in `\` or `/` create directory entries; all others create file entries.

Entry paths, _arguments of the `-EntryPath` parameter_, are always normalized, a few examples of how paths are normalized:

| Input                          | Normalized As               |
| ------------------------------ | --------------------------- |
| `C:\path\to\mynewentry.ext`    | `path/to/mynewentry.ext`    |
| `\\path\to\newdirectory\`      | `path/to/newdirectory/`     |
| `path\to\very/\random\/path\\` | `path/to/very/random/path/` |

> [!TIP]
> The `[PSCompression.Extensions.PathExtensions]::NormalizePath(string path)` static method is available as a public API if you want to normalize your paths before creating new entries.

When adding from string input (`-Value`), the `-EntryPath` parameter is required. When adding from a file (`-SourcePath`), `-EntryPath` is optional — if omitted, the normalized full path of the source file is used.

> [!NOTE]
> Due to a .NET limitation, adding files larger than 2 GB to an existing zip archive may fail. To handle such files, recreate the zip archive or use tools like 7-Zip. See [issue #19](https://github.com/santisq/PSCompression/issues/19) for details.

## EXAMPLES

### Example 1: Create empty entries

```powershell
PS ..\pwsh> New-ZipEntry .\test.zip -EntryPath test\entry, newfolder\

   Directory: /newfolder/

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Directory          2/24/2024  3:22 PM         0.00  B         0.00  B newfolder

   Directory: /test/

Type                    LastWriteTime  CompressedSize            Size Name
----                    -------------  --------------            ---- ----
Archive            2/24/2024  3:22 PM         0.00  B         0.00  B entry
```

This example creates an empty file entry (`test/entry`) and a directory entry (`newfolder/`) in `test.zip`.

### Example 2: Create entries with content from input strings

```powershell
PS ..\pwsh> 'hello', 'world', '!' | New-ZipEntry .\test.zip -EntryPath test\entry, newfolder\
New-ZipEntry: An entry with path 'test/entry' already exists in 'path\to\test.zip'.
New-ZipEntry: An entry with path 'newfolder/' already exists in 'path\to\test.zip'.

PS ..\pwsh> 'hello', 'world', '!' | New-ZipEntry .\test.zip -EntryPath test\entry, newfolder\ -Force
PS ..\pwsh> Get-ZipEntry .\test.zip -Include test/entry | Get-ZipEntryContent
hello
world
!
```

This example pipes three strings into `New-ZipEntry`, creating/overwriting file and directory entries. The content of the file entry (`test/entry`) becomes the piped strings (joined with newlines).

> [!TIP]
> The cmdlet prevents creating duplicate entries in the destination zip archive. Use `-Force` to overwrite existing entries with the same path.

### Example 3: Create entries with content from a source file path

```powershell
PS ..\pwsh> $file = 'hello world!' | New-Item mytestfile.txt
PS ..\pwsh> New-ZipEntry .\test.zip -SourcePath $file.FullName -EntryPath newentry.txt
```

This example adds the contents of a local file to the zip archive under the specified entry path.

### Example 4: Archive all files in a specified location

```powershell
PS ..\pwsh> $files = Get-ChildItem -File -Recurse
PS ..\pwsh> $files | ForEach-Object { New-ZipEntry .\test.zip -SourcePath $_.FullName }
```

This example recursively adds all files from the current directory to the zip archive, preserving their relative paths.

> [!TIP]
> The `-EntryPath` parameter is optional when using `-SourcePath`. If omitted, the normalized full path of the source file is used as the entry path.

### Example 5: Archive all `.txt` files in a specified location using a specified encoding

```powershell
PS ..\pwsh> $files = Get-ChildItem -File -Recurse -Filter *.txt
PS ..\pwsh> $files | ForEach-Object {
   $_ | Get-Content -Encoding ascii |
      New-ZipEntry .\test.zip -EntryPath $_.FullName -Encoding ascii
}
```

This example reads `.txt` files with a specific encoding and adds them to the zip archive under their original (normalized) paths.

> [!NOTE]
> When creating entries from piped string input (`-Value`), the `-EntryPath` parameter is required. Absolute paths are automatically normalized as shown in the Description section.

## PARAMETERS

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

### -Destination

Specifies the path to a Zip file where to create the entries. You can specify either a relative or an absolute path. A relative path is interpreted as relative to the current working directory. Note that the value is used exactly as it's typed. No characters are interpreted as wildcards.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Encoding

The character encoding used to set the entry content.

> [!NOTE]
>
> - This parameter applies only when content is provided via `-Value` (string input).
> - The default encoding is UTF-8 without BOM.

```yaml
Type: Encoding
Parameter Sets: Value
Aliases:

Required: False
Position: Named
Default value: utf8NoBOM
Accept pipeline input: False
Accept wildcard characters: False
```

### -EntryPath

Specifies the path to one or more entries to create in the destination Zip file. __The Type of the created entries is determined by their path__, for example, if the path ends with `\` or `/`, the entry will be created as a `Directory` entry, otherwise it will be an `Archive` entry.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force

Overwrites existing entries with the same path in the destination zip archive. Without `-Force`, the cmdlet throws an error if a duplicate path is encountered.

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

### -SourcePath

The path to the file to be archived. You can specify either a relative or an absolute path. A relative path is interpreted as relative to the current working directory.

```yaml
Type: String
Parameter Sets: File
Aliases:

Required: False
Position: 2
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Value

The string content that will be set to the created entries. You can also pipe a value to `New-ZipEntry`.

```yaml
Type: String[]
Parameter Sets: Value
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

You can pipe one or more strings to this cmdlet to use as content for new file entries (requires `-EntryPath`).

## OUTPUTS

### PSCompression.ZipEntryDirectory

### PSCompression.ZipEntryFile

The cmdlet outputs the newly created `ZipEntryDirectory` or `ZipEntryFile` objects.

## NOTES

## RELATED LINKS

[__System.IO.Compression.ZipArchive__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive)

[__System.IO.Compression.ZipArchiveEntry__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchiveentry)
