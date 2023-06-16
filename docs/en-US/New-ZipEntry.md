---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# New-ZipEntry

## SYNOPSIS

Creates Zip Archive Entries from one or more specified entry relative paths.

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

The `New-ZipEntry` cmdlet can create one or more Zip Archive Entries from specified paths. The type of the created entries is determined by their path, for example, if a path ends with `\` or `/`, the entry will be created as a `Directory` entry, otherwise it will be an `Archive` entry.

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

### Example 4: Archive all files in a specified location

```powershell
PS ..\pwsh> $files = Get-ChildItem -File -Recurse
PS ..\pwsh> $files | ForEach-Object {
   New-ZipEntry .\test.zip -EntryPath $_.FullName.Remove(0, $pwd.Path.Length) -SourcePath $_.FullName
}
```

In this example `$_.FullName.Remove(0, $pwd.Path.Length)` is used to get the file paths relative to the current location. Using `-EntryPath $_.FullName` without getting the relative paths would work too however this would cause issues while attempting to extract the files later.

### Example 5: Archive all `.txt` files in a specified location using a specified encoding

```powershell
PS ..\pwsh> $files = Get-ChildItem -File -Recurse -Filter *.txt
PS ..\pwsh> $files | ForEach-Object {
   $_ | Get-Content -Encoding ascii |
      New-ZipEntry .\test.zip -EntryPath $_.FullName.Remove(0, $pwd.Path.Length) -Encoding ascii
}
```

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

The character encoding used to set the entry content. __This parameter is applicable only when `-SourcePath` is not used.__ The default encoding is __`utf8NoBOM`__.

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

The cmdlet prevents creating entries in a destination Zip archive if an entry with the same relative path already exists. You can use the `-Force` parameter to overwrite them.

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

### String

You can pipe a value for the new zip entry to this cmdlet.

## OUTPUTS

### ZipEntryDirectory

### ZipEntryFile
