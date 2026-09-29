---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Get-RarEntryContent

## SYNOPSIS

Retrieves the content of one or more file entries from a RAR archive.

## SYNTAX

### Stream (Default)

```powershell
Get-RarEntryContent
    -Entry <RarEntry[]>
    [-Encoding <Encoding>]
    [-Raw]
    [-Password <SecureString>]
    [<CommonParameters>]
```

### Bytes

```powershell
Get-RarEntryContent
    -Entry <RarEntry[]>
    [-Raw]
    [-AsByteStream]
    [-BufferSize <Int32>]
    [-Password <SecureString>]
    [<CommonParameters>]
```

## DESCRIPTION

The `Get-RarEntryContent` cmdlet retrieves the content of `RarEntry` objects produced by `Get-RarEntry`. This cmdlet supports text output (line-by-line or raw string) and binary output (byte arrays or streams). It also supports reading password-protected entries.

> [!TIP]
> Entries output by `Get-RarEntry` can be piped directly to this cmdlet.

## EXAMPLES

### Example 1: Get the content of a RAR archive entry

```powershell
PS /> Get-RarEntry .\archive.rar -Include folder1/file1.txt | Get-RarEntryContent
```

This example retrieves the text content of a specific file entry from a RAR archive. By default, content is streamed line by line (as an array of strings).

### Example 2: Get raw content of a RAR archive entry

```powershell
PS /> Get-RarEntry .\archive.rar -Include folder1/file1.txt | Get-RarEntryContent -Raw
```

This example retrieves the entire text content as a single multi-line string using the `-Raw` switch.

### Example 3: Get the bytes of a RAR archive entry as a stream

```powershell
PS /> $bytes = Get-RarEntry .\archive.rar -Include folder1/helloworld.txt | Get-RarEntryContent -AsByteStream
PS /> [System.Text.Encoding]::UTF8.GetString($bytes)
hello world!
```

This example retrieves the raw bytes of a file entry as a byte array using `-AsByteStream`, then converts them to a string.

### Example 4: Get contents of all .md files as byte arrays

```powershell
PS /> $bytes = Get-RarEntry .\archive.rar -Include *.md | Get-RarEntryContent -AsByteStream -Raw
PS /> $bytes[0].GetType()

IsPublic IsSerial Name                                     BaseType
-------- -------- ----                                     --------
True     True     Byte[]                                   System.Array

PS /> $bytes[1].Length
7767
```

This example retrieves the raw bytes of all `.md` files as an array of `byte[]` objects (one per entry) using `-AsByteStream` and `-Raw`.

### Example 5: Get content from an input stream

```powershell
PS /> $stream = Invoke-WebRequest https://example.com/archive.rar
PS /> $stream | Get-RarEntry -Include readme.md | Get-RarEntryContent -Raw
```

This example retrieves the content of `readme.md` from a RAR archive streamed from the web.

### Example 6: Get content from a password-protected entry

```powershell
PS /> Get-RarEntry .\archive.rar -Include secret.txt | Get-RarEntryContent -Password (Read-Host -AsSecureString)
```

This example demonstrates how to read an encrypted entry using `Read-Host -AsSecureString` to provide the password.

> [!TIP]
> If an entry is encrypted and no password is supplied, the cmdlet will prompt for one interactively.

## PARAMETERS

### -AsByteStream

Specifies that the content should be read as a stream of bytes.

> [!TIP]
> By default, `-AsByteStream` streams bytes through the pipeline one by one. To receive the entire un-enumerated byte array (`byte[]`) per entry without pipeline unrolling, combine it with `-Raw`.

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
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Encoding

Specifies the character encoding used to read the entry content. The default encoding is `utf8NoBOM`.

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
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Entry

The RAR entry or entries to get the content from. This parameter accepts pipeline input from `Get-RarEntry` but can also be used as a named parameter.

```yaml
Type: RarEntry[]
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Password

Specifies the password as a `SecureString` to read the encrypted RAR entry.

> [!NOTE]
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

### PSCompression.FormatHandlers.Rar.RarEntry[]

You can pipe one or more `RarEntry` objects produced by [`Get-RarEntry`](Get-RarEntry.md) to this cmdlet.

## OUTPUTS

### System.String

By default, this cmdlet returns the content as an array of strings, one per line. When the `-Raw` parameter is used, it returns a single string.

### System.Byte

Outputs bytes to the pipeline when `-AsByteStream` is specified.

- __`-AsByteStream`__: Streams the bytes individually one by one (enumerating the entry's byte array).
- __`-AsByteStream` and `-Raw`__: Outputs the raw byte array (`byte[]`) for each entry without enumeration.

## NOTES

## RELATED LINKS

[__`Get-RarEntry`__](Get-RarEntry.md)

[__`Expand-RarEntry`__](Expand-RarEntry.md)

[__SharpCompress__](https://github.com/adamhathcock/sharpcompress)
