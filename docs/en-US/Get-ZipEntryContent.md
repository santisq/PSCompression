---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Get-ZipEntryContent

## SYNOPSIS

Gets the content of a zip entry.

## SYNTAX

### Stream (Default)

```powershell
Get-ZipEntryContent
    -ZipEntry <ZipEntryFile[]>
    [-Encoding <Encoding>]
    [-Raw]
    [<CommonParameters>]
```

### Bytes

```powershell
Get-ZipEntryContent
    -ZipEntry <ZipEntryFile[]>
    [-Raw]
    [-AsByteStream]
    [-BufferSize <Int32>]
    [<CommonParameters>]
```

## DESCRIPTION

The `Get-ZipEntryContent` cmdlet gets the content of one or more `ZipEntryFile` instances.
This cmdlet is meant to be used with [`Get-ZipEntry`](./Get-ZipEntry.md) as your entry point.

> [!TIP]
> Entries outputted by `Get-ZipEntry` can be piped to this cmdlet.

## EXAMPLES

### Example 1: Get the content of a Zip Archive Entry

```powershell
PS ..pwsh\> Get-ZipEntry .\myZip.zip -Include myrelative/entry.txt | Get-ZipEntryContent
```

`-Include` parameter from `Get-ZipEntry` can be used to target a specific entry by passing the entry's relative path, from there the output can be piped directly to `Get-ZipEntryContent`.
By default, the cmdlet streams line-by-line .

### Example 2: Get raw content of a Zip Archive Entry

```powershell
PS ..pwsh\> Get-ZipEntry .\myZip.zip -Include myrelative/entry.txt | Get-ZipEntryContent -Raw
```

The cmdlet outputs a single multi-line string when the `-Raw` switch is used instead of line-by-line streaming.

### Example 3: Get the bytes of a Zip Archive Entry as a Stream

```powershell
PS ..pwsh\> $bytes = Get-ZipEntry .\test.zip -Include test/helloworld.txt | Get-ZipEntryContent -AsByteStream
PS ..pwsh\> $bytes
104
101
108
108
111
32
119
111
114
108
100
33
13
10

PS ..pwsh\> [System.Text.Encoding]::UTF8.GetString($bytes)
hello world!
```

The `-AsByteStream` switch can be useful to read non-text zip entries.

### Example 4: Get contents of all `.md` files as byte arrays

```powershell
PS ..pwsh\> $bytes = Get-ZipEntry .\test.zip -Include *.md | Get-ZipEntryContent -AsByteStream -Raw
PS ..pwsh\> $bytes[0].GetType()

IsPublic IsSerial Name                                     BaseType
-------- -------- ----                                     --------
True     True     Byte[]                                   System.Array

PS ..pwsh\> $bytes[1].Length
7767
```

When the `-Raw` and `-AsByteStream` switches are used together the cmdlet outputs `byte[]` as single objects for each zip entry.

### Example 5: Get content from input Stream

```powershell
PS ..\pwsh> $package = Invoke-WebRequest https://www.powershellgallery.com/api/v2/package/PSCompression
PS ..\pwsh> $package | Get-ZipEntry -Include *.psd1 | Get-ZipEntryContent -Raw | Invoke-Expression

Name                           Value
----                           -----
PowerShellVersion              5.1
Description                    Zip and GZip utilities for PowerShell!
RootModule                     bin/netstandard2.0/PSCompression.dll
FormatsToProcess               {PSCompression.Format.ps1xml}
VariablesToExport              {}
PrivateData                    {[PSData, System.Collections.Hashtable]}
CmdletsToExport                {Get-ZipEntry, Get-ZipEntryContent, Set-ZipEntryContent, Remove-ZipEntry…}
ModuleVersion                  2.0.10
Author                         Santiago Squarzon
CompanyName                    Unknown
GUID                           c63aa90e-ae64-4ae1-b1c8-456e0d13967e
FunctionsToExport              {}
RequiredAssemblies             {System.IO.Compression, System.IO.Compression.FileSystem}
Copyright                      (c) Santiago Squarzon. All rights reserved.
AliasesToExport                {gziptofile, gzipfromfile, gziptostring, gzipfromstring…}
```

## PARAMETERS

### -BufferSize

This parameter determines the total number of bytes read into the buffer before outputting the stream of bytes. __This parameter is applicable only when `-Raw` is not used.__ The buffer default value is __128 KiB.__

```yaml
Type: Int32
Parameter Sets: Bytes
Aliases:

Required: False
Position: Named
Default value: 128000
Accept pipeline input: False
Accept wildcard characters: False
```

### -Encoding

The character encoding used to read the entry content.

> [!NOTE]
>
> - __This parameter is applicable only when `-AsByteStream` is not used.__
> - The default encoding is __`utf8NoBOM`__.

```yaml
Type: Encoding
Parameter Sets: Stream
Aliases:

Required: False
Position: Named
Default value: utf8NoBOM
Accept pipeline input: False
Accept wildcard characters: False
```

### -Raw

Ignores newline characters and returns the entire contents of an entry in one string with the newlines preserved. By default, newline characters in a file are used as delimiters to separate the input into an array of strings.

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

### -ZipEntry

The entry or entries to get the content from. This parameter can be and is meant to be bound from pipeline however can be also used as a named parameter.

```yaml
Type: ZipEntryFile[]
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -AsByteStream

Specifies that the content should be read as a stream of bytes.

```yaml
Type: SwitchParameter
Parameter Sets: Bytes
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters. See [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### ZipEntryFile

You can pipe instances of `ZipEntryFile` to this cmdlet. These instances are produced by [`Get-ZipEntry`](Get-ZipEntry.md) and [`New-ZipEntry`](New-ZipEntry.md) cmdlets.

## OUTPUTS

### String

By default, this cmdlet returns the content as an array of strings, one per line. When the `-Raw` parameter is used, it returns a single string.

### Byte

This cmdlet returns the content as bytes when the `-AsByteStream` parameter is used.
