---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Get-TarEntryContent

## SYNOPSIS

Retrieves the content of one or more file entries from a tar archive.

## SYNTAX

### Stream (Default)

```powershell
Get-TarEntryContent
    -Entry <TarEntryFile[]>
    [-Encoding <Encoding>]
    [-Raw]
    [<CommonParameters>]
```

### Bytes

```powershell
Get-TarEntryContent
    -Entry <TarEntryFile[]>
    [-Raw]
    [-AsByteStream]
    [-BufferSize <Int32>]
    [<CommonParameters>]
```

## DESCRIPTION

The `Get-TarEntryContent` cmdlet retrieves the content of `TarEntryFile` objects produced by [`Get-TarEntry`](./Get-TarEntry.md). This cmdlet supports text output (line-by-line or raw string) and binary output (byte arrays or streams).

> [!TIP]
> Entries output by `Get-TarEntry` can be piped to this cmdlet.

## EXAMPLES

### Example 1: Get the content of a tar archive entry

```powershell
PS C:\> Get-TarEntry .\archive.tar -Include folder1/file1.txt | Get-TarEntryContent
```

This example retrieves the text content of a specific file entry from a tar archive. By default, content is streamed line by line (as an array of strings).

### Example 2: Get raw content of a tar archive entry

```powershell
PS C:\> Get-TarEntry .\archive.tar -Include folder1/file1.txt | Get-TarEntryContent -Raw
```

This example retrieves the entire text content as a single multi-line string using the `-Raw` switch.

### Example 3: Get the bytes of a tar archive entry as a stream

```powershell
PS C:\> $bytes = Get-TarEntry .\archive.tar -Include folder1/helloworld.txt | Get-TarEntryContent -AsByteStream
PS C:\> [System.Text.Encoding]::UTF8.GetString($bytes)
hello world!
```

This example retrieves the raw bytes of a file entry as a byte array using `-AsByteStream`, then converts them to a string.

### Example 4: Get contents of all `.md` files as byte arrays

```powershell
PS C:\> $bytes = Get-TarEntry .\archive.tar.gz -Algorithm gz -Include *.md | Get-TarEntryContent -AsByteStream -Raw
PS C:\> $bytes[0].GetType()

IsPublic IsSerial Name                                     BaseType
-------- -------- ----                                     --------
True     True     Byte[]                                   System.Array

PS C:\> $bytes[1].Length
7767
```

This example retrieves the raw bytes of all `.md` files as an array of `byte[]` objects (one per entry) using `-AsByteStream` and `-Raw`.

### Example 5: Get content from an input stream

```powershell
PS C:\> $stream = Invoke-WebRequest https://example.com/archive.tar.gz
PS C:\> $stream | Get-TarEntry -Include readme.md -Algorithm gz | Get-TarEntryContent -Raw
```

This example retrieves the content of `readme.md` from a gzip-compressed tar archive streamed from the web.

> [!NOTE]
> When `Get-TarEntry` processes a stream, it defaults to the `gz` (gzip) algorithm. Specify `-Algorithm` (e.g., `-Algorithm bz2` for bzip2) to match the compression type, or an error may occur if the stream is not gzip-compressed.

## PARAMETERS

### -AsByteStream

Specifies that the content should be read as a stream of bytes.

```yaml
Type: SwitchParameter
Parameter Sets: Bytes
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

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

### -Entry

The tar entry or entries to get the content from. This parameter is designed to accept pipeline input from `Get-TarEntry` but can also be used as a named parameter.

```yaml
Type: TarEntryFile[]
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
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
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### PSCompression.TarEntryFile[]

You can pipe one or more `TarEntryFile` objects produced by [`Get-TarEntry`](./Get-TarEntry.md) to this cmdlet.

## OUTPUTS

### System.String

By default, this cmdlet returns the content as an array of strings, one per line. When the `-Raw` parameter is used, it returns a single string.

### System.Byte

- When the `-AsByteStream` parameter is used, this cmdlet returns the content as a byte array (`System.Byte[]`).
- When `-AsByteStream` and `-Raw` are combined, it returns an array of byte arrays (one per entry).

## NOTES

## RELATED LINKS

[__`Get-TarEntry`__](./Get-TarEntry.md)

[__`Expand-TarEntry`__](./Expand-TarEntry.md)

[__SharpZipLib__](https://github.com/icsharpcode/SharpZipLib)

[__SharpCompress__](https://github.com/adamhathcock/sharpcompress)

[__ZstdSharp__](https://github.com/oleg-st/ZstdSharp)
