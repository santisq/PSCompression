---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Set-ZipEntryContent

## SYNOPSIS

Sets or appends content to an existing zip file entry.

## SYNTAX

### StringValue (Default)

```powershell
Set-ZipEntryContent
    -Value <Object[]>
    [-SourceEntry] <ZipEntryFile>
    [-Encoding <Encoding>]
    [-Append]
    [-PassThru]
    [<CommonParameters>]
```

### ByteStream

```powershell
Set-ZipEntryContent
    -Value <Object[]>
    [-SourceEntry] <ZipEntryFile>
    [-AsByteStream]
    [-Append]
    [-BufferSize <Int32>]
    [-PassThru]
    [<CommonParameters>]
```

## DESCRIPTION

The `Set-ZipEntryContent` cmdlet writes or appends content to a `ZipEntryFile` object produced by [`Get-ZipEntry`](./Get-ZipEntry.md) or [`New-ZipEntry`](./New-ZipEntry.md). By default, it replaces existing content; use `-Append` to add to it. Content can be text (strings) or raw bytes (via `-AsByteStream`). Input can be piped or provided via `-Value`.

To create a new entry, use [`New-ZipEntry`](./New-ZipEntry.md).

> [!NOTE]
> Due to a .NET limitation, writing or appending content larger than 2 GB to an existing zip entry may fail. To handle such content, recreate the zip archive or use tools like 7-Zip. See [issue #19](https://github.com/santisq/PSCompression/issues/19) for details.

## EXAMPLES

### Example 1: Write new content to a Zip Archive Entry

```powershell
PS ..pwsh\> $entry = New-ZipEntry .\test.zip -EntryPath test\helloworld.txt
PS ..pwsh\> 'hello', 'world', '!' | Set-ZipEntryContent $entry
PS ..pwsh\> $entry | Get-ZipEntryContent
hello
world
!
```

This example creates a new file entry and pipes strings to set its content (replacing any existing data).

### Example 2: Append content to a Zip Archive Entry

```powershell
PS ..pwsh\> Set-ZipEntryContent $entry -Value 'hello', 'world', '!' -Append
PS ..pwsh\> $entry | Get-ZipEntryContent
hello
world
!
hello
world
!
```

This example appends additional strings to the existing entry content using `-Append`.

### Example 3: Write raw bytes to a Zip Archive Entry

```powershell
PS ..pwsh\> $entry = Get-ZipEntry .\test.zip -Include test/helloworld.txt
PS ..pwsh\> $bytes = [System.Text.Encoding]::UTF8.GetBytes('hello world!')
PS ..pwsh\> $bytes | Set-ZipEntryContent $entry -AsByteStream
PS ..pwsh\> $entry | Get-ZipEntryContent
hello world!
```

This example appends the same byte array to the entry using `-AsByteStream` and `-Append`.

### Example 4: Append raw bytes to a Zip Archive Entry

```powershell
PS ..pwsh\> $bytes | Set-ZipEntryContent $entry -AsByteStream -Append
PS ..pwsh\> $entry | Get-ZipEntryContent
hello world!hello world!
```

Using the same byte array in the previous example, we can append bytes to the entry stream.

## PARAMETERS

### -Append

Appends the content to the zip entry instead of overwriting it.

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

### -AsByteStream

Specifies that the content should be written as a stream of bytes.

```yaml
Type: SwitchParameter
Parameter Sets: ByteStream
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -BufferSize

For efficiency purposes this cmdlet buffers bytes before writing them to the Zip Archive Entry. This parameter determines how many bytes are buffered before being written to the stream.

> [!NOTE]
>
> - This parameter applies only when `-AsByteStream` is used.
> - The default buffer size is __128 KiB.__

```yaml
Type: Int32
Parameter Sets: ByteStream
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
> - This parameter applies only when `-AsByteStream` is not used.
> - The default encoding is UTF-8 without BOM.

```yaml
Type: Encoding
Parameter Sets: StringValue
Aliases:

Required: False
Position: Named
Default value: utf8NoBOM
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru

Outputs the updated `ZipEntryFile` object. By default, the cmdlet produces no output.

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

### -SourceEntry

Specifies the zip archive entry that receives the content. `ZipEntryFile` instances can be obtained using `Get-ZipEntry` or `New-ZipEntry` cmdlets.

```yaml
Type: ZipEntryFile
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Value

Specifies the new content for the zip entry.

```yaml
Type: Object[]
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.Object[]

You can pipe strings (for text content) or byte arrays (for binary content) to this cmdlet.

## OUTPUTS

### None

By default, this cmdlet produces no output.

### PSCompression.ZipEntryFile

When the `-PassThru` switch is used, the cmdlet outputs the updated `ZipEntryFile` object.

## NOTES

## RELATED LINKS

[__`Get-ZipEntry`__](./Get-ZipEntry.md)

[__`New-ZipEntry`__](./New-ZipEntry.md)

[__System.IO.Compression.ZipArchive__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive)

[__System.IO.Compression.ZipArchiveEntry__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchiveentry)
