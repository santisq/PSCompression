---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Get-ZipEntryContent

## SYNOPSIS

Retrieves the content of one or more file entries from a zip archive.

## SYNTAX

### Stream (Default)

```powershell
Get-ZipEntryContent
    -Entry <ZipEntryFile[]>
    [-Encoding <Encoding>]
    [-Raw]
    [-Password <SecureString>]
    [<CommonParameters>]
```

### Bytes

```powershell
Get-ZipEntryContent
    -Entry <ZipEntryFile[]>
    [-Raw]
    [-AsByteStream]
    [-BufferSize <Int32>]
    [-Password <SecureString>]
    [<CommonParameters>]
```

## DESCRIPTION

The `Get-ZipEntryContent` cmdlet retrieves the content of `ZipEntryFile` objects produced by [`Get-ZipEntry`](./Get-ZipEntry.md) or [`New-ZipEntry`](./New-ZipEntry.md). This cmdlet supports text output (line-by-line or raw string) and binary output (byte arrays or streams). It also supports reading password-protected entries.

> [!TIP]
> Entries outputted by `Get-ZipEntry` can be piped to this cmdlet.

## EXAMPLES

### Example 1: Get the content of a Zip Archive Entry

```powershell
PS ..pwsh\> Get-ZipEntry .\myZip.zip -Include myrelative/entry.txt | Get-ZipEntryContent
```

This example retrieves the text content of a specific file entry from a zip archive. By default, content is streamed line by line (as an array of strings).

### Example 2: Get raw content of a Zip Archive Entry

```powershell
PS ..pwsh\> Get-ZipEntry .\myZip.zip -Include myrelative/entry.txt | Get-ZipEntryContent -Raw
```

This example retrieves the entire text content as a single multi-line string using the `-Raw` switch.

### Example 3: Get the bytes of a Zip Archive Entry as a Stream

```powershell
PS ..pwsh\> $bytes = Get-ZipEntry .\test.zip -Include test/helloworld.txt | Get-ZipEntryContent -AsByteStream
PS ..pwsh\> [System.Text.Encoding]::UTF8.GetString($bytes)
hello world!
```

This example retrieves the raw bytes of a file entry as a byte array using `-AsByteStream`, then converts them to a string.

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

This example retrieves the raw bytes of all `.md` files as an array of `byte[]` objects (one per entry) using `-AsByteStream` and `-Raw`.

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

This example downloads a NuGet package (a zip archive) from PowerShell Gallery, extracts the manifest (`.psd1`) file, and immediately evaluates it to display module metadata.

### Example 6: Get content from a password protected entry

```powershell
PS ..\pwsh> Get-ZipEntry .\myZip.zip -Include myEncryptedEntry.txt | Get-ZipEntryContent -Password (Read-Host -AsSecureString)
```

This example demonstrates how to read an encrypted entry using `Read-Host -AsSecureString` to provide the password.

> [!TIP]
> If an entry is encrypted and no password is supplied, the cmdlet will prompt for one.

## PARAMETERS

### -BufferSize

Determines the number of bytes read into the buffer before outputting the stream of bytes. This parameter applies only when `-Raw` is not used. The default buffer size is 128 KiB.

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

Specifies the character encoding used to read the entry content. . The default encoding is `utf8NoBOM`.

> [!NOTE]
>
> - This parameter applies only when `-AsByteStream` is not used.
> - The default encoding is UTF-8 without BOM.

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

By default, the cmdlet outputs text content as an array of strings (split on newlines). The `-Raw` switch returns the entire content as a single string with newlines preserved.

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

### -Entry

The zip entry or entries to get the content from. This parameter is designed to accept pipeline input from `Get-ZipEntry` but can also be used as a named parameter.

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

### -Password

Specifies the password as a `SecureString` to extract the encrypted zip entry.

> [!TIP]
> If an entry is encrypted and no password is supplied, the cmdlet will prompt for one.

```yaml
Type: SecureString
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters. See [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### PSCompression.ZipEntryFile[]

You can pipe one or more `ZipEntryFile` objects produced by [`Get-ZipEntry`](./Get-ZipEntry.md) or [`New-ZipEntry`](./New-ZipEntry.md) to this cmdlet.

## OUTPUTS

### System.String

By default, this cmdlet returns the content as an array of strings, one per line. When the `-Raw` parameter is used, it returns a single string.

### System.Byte

- When the `-AsByteStream` parameter is used, this cmdlet returns the content as a byte array (`System.Byte[]`).
- When `-AsByteStream` and `-Raw` are combined, it returns an array of byte arrays (one per entry).

## NOTES

## RELATED LINKS

[__`Get-ZipEntry`__](./Get-ZipEntry.md)

[__SharpZipLib__](https://github.com/icsharpcode/SharpZipLib)
