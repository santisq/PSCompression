---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# New-ZipEntry

## SYNOPSIS

Creates new Zip Archive Entries from one or more specified entry relative paths.

## SYNTAX

### Value (Default)

```powershell
New-ZipEntry [-Value <String[]>] -Destination <String> -EntryPath <String[]>
 [-CompressionLevel <CompressionLevel>] [-Encoding <Encoding>] [-Force] [<CommonParameters>]
```

### File

```powershell
New-ZipEntry -Destination <String> -EntryPath <String[]> [-SourcePath <String>]
 [-CompressionLevel <CompressionLevel>] [-Force] [<CommonParameters>]
```

## DESCRIPTION

The `New-ZipEntry` cmdlet can create one or more Zip Archive Entries from specified paths. The type of the newly created entries are determined by their path, for example, if a path ends with `\` or `/`, the entry will be created as a `Directory` entry, otherwise it will be an `Archive` entry.

Entry paths (arguments of the `-EntryPath` parameter) are always normalized, a few examples of how paths are normalized:

| Input | Normalized As |
| --- | --- |
| `path\to\mynewentry.ext` | `path/to/mynewentry.ext` |
| `\path\to\newdirectory\` | `path/to/newdirectory/` |
| `path\to\very/\random\/path\\` | `path/to/very/random/path/` |

The `[PSCompression.Extensions]::NormalizePath(string path)` static method is available as a public API if you would like to normalize your paths before creating new entries.

In addition, `New-ZipEntry` can set the content of the entries that it creates from string input or by specifying a source file path.

## EXAMPLES

### Example 1: Create empty entries

```powershell
PS ..\pwsh> New-ZipEntry .\test.zip -EntryPath test\entry, newfolder\

   Directory: newfolder/

EntryType               LastWriteTime  CompressedSize            Size EntryName
---------               -------------  --------------            ---- ---------
Directory          6/11/2023  6:55 PM         0.00  B         0.00  B

   Directory: test/

EntryType               LastWriteTime  CompressedSize            Size EntryName
---------               -------------  --------------            ---- ---------
Archive            6/11/2023  6:55 PM         0.00  B         0.00  B entry
```

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

The cmdlet prevents creating entries in a destination Zip archive if an entry with the same relative path already exists. You can use the `-Force` parameter to overwrite them.

### Example 3: Create entries with content from a source file path

```powershell
PS ..\pwsh> $file = 'hello world!' | New-Item mytestfile.txt
PS ..\pwsh> New-ZipEntry .\test.zip -EntryPath newentry.txt -SourcePath $file.FullName
```

### Example 4: Add all files in a specified location with their content

```powershell
PS ..\pwsh> $files = Get-ChildItem -File -Recurse
PS ..\pwsh> $files | ForEach-Object { New-ZipEntry .\test.zip -EntryPath $_.FullName.Remove(0, $pwd.Path.Length) -SourcePath $_.FullName }
```

In this example `$_.FullName.Remove(0, $pwd.Path.Length)` is used to get the file paths relative to the current location. Using `-EntryPath $_.FullName` without getting the relative paths would work too however this would cause issues while attempting to extract the files later.

### Example 5: Add all `.txt` files in a specified location with their content using a specified encoding

```powershell
PS ..\pwsh> $files = Get-ChildItem -File -Recurse -Filter *.txt
PS ..\pwsh> $files | ForEach-Object { $_ | Get-Content -Encoding ascii | New-ZipEntry .\test.zip -EntryPath $_.FullName.Remove(0, $pwd.Path.Length) -Encoding ascii }
```

## PARAMETERS

### -CompressionLevel
{{ Fill CompressionLevel Description }}

```yaml
Type: CompressionLevel
Parameter Sets: (All)
Aliases:
Accepted values: Optimal, Fastest, NoCompression, SmallestSize

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Destination
{{ Fill Destination Description }}

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Encoding
{{ Fill Encoding Description }}

```yaml
Type: Encoding
Parameter Sets: Value
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -EntryPath
{{ Fill EntryPath Description }}

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force
{{ Fill Force Description }}

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
{{ Fill SourcePath Description }}

```yaml
Type: String
Parameter Sets: File
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Value
{{ Fill Value Description }}

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
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### None
## OUTPUTS

### PSCompression.ZipEntryDirectory
### PSCompression.ZipEntryDirectory
## NOTES

## RELATED LINKS
