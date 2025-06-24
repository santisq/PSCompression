---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Get-TarEntryContent

## SYNOPSIS

Gets the content of a tar archive entry.

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

The `Get-TarEntryContent` cmdlet retrieves the content of one or more `TarEntryFile` instances. This cmdlet is designed to be used with [`Get-TarEntry`](./Get-TarEntry.md) as the entry point.

> [!TIP]
> Entries output by `Get-TarEntry` can be piped to this cmdlet.

## EXAMPLES

### Example 1: Get the content of a tar archive entry

```powershell
PS C:\> Get-TarEntry .\archive.tar -Include folder1/file1.txt | Get-TarEntryContent

Line 1 of file1.txt
Line 2 of file1.txt
```

The `-Include` parameter from `Get-TarEntry` targets a specific entry by its relative path, and the output is piped to `Get-TarEntryContent`. By default, the cmdlet streams content line by line.

### Example 2: Get raw content of a tar archive entry

```powershell
PS C:\> Get-TarEntry .\archive.tar -Include folder1/file1.txt | Get-TarEntryContent -Raw

Line 1 of file1.txt
Line 2 of file1.txt
```

The cmdlet outputs a single multi-line string when the `-Raw` switch is used instead of line-by-line streaming.

### Example 3: Get the bytes of a tar archive entry as a stream

```powershell
PS C:\> $bytes = Get-TarEntry .\archive.tar -Include folder1/helloworld.txt | Get-TarEntryContent -AsByteStream
PS C:\> $bytes
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

PS C:\> [System.Text.Encoding]::UTF8.GetString($bytes)
hello world!
```

The `-AsByteStream` switch is useful for reading non-text tar entries.

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

When the `-Raw` and `-AsByteStream` switches are used together, the cmdlet outputs `byte[]` as single objects for each tar entry.

### Example 5: Get content from an input stream

```powershell
PS C:\> $stream = Invoke-WebRequest https://example.com/archive.tar.gz
PS C:\> $content = $stream | Get-TarEntry -Include readme.md -Algorithm gz | Get-TarEntryContent -Raw
PS C:\> $content

# My Project
This is the README file for my project.
```

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

Returns the entire contents of an entry as a single string with newlines preserved, ignoring newline characters. By default, newline characters are used to separate the content into an array of strings.

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

You can pipe instances of `TarEntryFile` to this cmdlet, produced by [`Get-TarEntry`](./Get-TarEntry.md).

## OUTPUTS

### System.String

By default, this cmdlet returns the content as an array of strings, one per line. When the `-Raw` parameter is used, it returns a single string.

### System.Byte

This cmdlet returns the content as bytes when the `-AsByteStream` parameter is used.

## NOTES

## RELATED LINKS

[__Get-TarEntry__](https://github.com/santisq/PSCompression)

[__Expand-TarEntry__](https://github.com/santisq/PSCompression)

[__SharpZipLib__](https://github.com/icsharpcode/SharpZipLib)

[__SharpCompress__](https://github.com/adamhathcock/sharpcompress)

[__ZstdSharp__](https://github.com/oleg-st/ZstdSharp)
