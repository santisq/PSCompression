---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Set-ZipEntryContent

## SYNOPSIS

Sets or appends content to an existing zip entry.

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

The `Set-ZipEntryContent` cmdlet can write or append content to a Zip Archive Entry. By default, this cmdlet replaces the existing content of a Zip Archive Entry, if you need to append content you can use the `-Append` switch. This cmdlet also supports writing or appending raw bytes while using the `-AsByteStream` switch. To send content to `Set-ZipEntryContent` you can use the `-Value` parameter on the command line or send content through the pipeline.

If you need to create a new Zip Archive Entry you can use the [`New-ZipEntry` cmdlet](./New-ZipEntry.md).

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

You can send content through the pipeline or using the `-Value` parameter as shown in the next example.

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

### Example 3: Write raw bytes to a Zip Archive Entry

```powershell
PS ..pwsh\> $entry = Get-ZipEntry .\test.zip -Include test/helloworld.txt
PS ..pwsh\> $bytes = [System.Text.Encoding]::UTF8.GetBytes('hello world!')
PS ..pwsh\> $bytes | Set-ZipEntryContent $entry -AsByteStream
PS ..pwsh\> $entry | Get-ZipEntryContent
hello world!
```

The cmdlet supports writing and appending raw bytes while using the `-AsByteStream` switch.

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
> - __This parameter is applicable only when `-AsByteStream` is used.__
> The buffer default value is __128 KiB.__

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
> - __This parameter is applicable only when `-AsByteStream` is not used.__
> - The default encoding is __`utf8NoBOM`__.

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

Outputs the object representing the updated zip archive entry. By default, this cmdlet does not generate any output.

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

### Object

You can pipe strings or bytes to this cmdlet.

## OUTPUTS

### None

This cmdlet produces no output by default .

### ZipEntryFile

This cmdlet outputs the updated entry when the `-PassThru` switch is used.
