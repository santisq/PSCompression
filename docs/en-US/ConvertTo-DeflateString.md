---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# ConvertTo-DeflateString

## SYNOPSIS

Compresses input strings into a Deflate-compressed Base64-encoded string.

## SYNTAX

```powershell
ConvertTo-DeflateString
    [-InputObject] <String[]>
    [-Encoding <Encoding>]
    [-CompressionLevel <CompressionLevel>]
    [-AsByteStream]
    [-NoNewLine]
    [<CommonParameters>]
```

## DESCRIPTION

The `ConvertTo-DeflateString` cmdlet compresses input strings into Deflate-compressed Base64-encoded strings or raw bytes using the [`DeflateStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.deflatestream). It is the counterpart to [`ConvertFrom-DeflateString`](./ConvertFrom-DeflateString.md).

## EXAMPLES

### Example 1: Compress strings to Deflate compressed Base64 encoded string

```powershell
PS ..\pwsh> $strings = 'hello', 'world', '!'
PS ..\pwsh> ConvertTo-DeflateString $strings

ykjNycnn5SrPL8pJ4eVS5OUCAAAA//8DAA==

# Or using pipeline input
PS ..\pwsh> $strings | ConvertTo-DeflateString

ykjNycnn5SrPL8pJ4eVS5OUCAAAA//8DAA==
```

This example shows how to compress an array of strings into a single Deflate-compressed Base64 string, using either argument or pipeline input.

### Example 2: Save Deflate-compressed bytes to a file using `-AsByteStream`

```powershell
PS ..\pwsh> 'hello world!' | ConvertTo-DeflateString -AsByteStream | Set-Content -FilePath .\helloworld.deflate -AsByteStream

# To read the file back you can use `ConvertFrom-DeflateString` following these steps:
PS ..\pwsh> $path = Convert-Path .\helloworld.deflate
PS ..\pwsh> [System.Convert]::ToBase64String([System.IO.File]::ReadAllBytes($path)) | ConvertFrom-DeflateString

hello world!
```

This example shows how to use `-AsByteStream` to output raw compressed bytes that can be written to a file using `Set-Content` or `Out-File`. Note that the byte array is not enumerated.

> [!NOTE]
> The example uses `-AsByteStream` with `Set-Content`, which is available in PowerShell 7+. In Windows PowerShell 5.1, use `-Encoding Byte` with `Set-Content` or `Out-File` to write the byte array to a file.

### Example 3: Compress strings using a specific Encoding

```powershell
PS ..\pwsh> 'ñ' | ConvertTo-DeflateString -Encoding ansi | ConvertFrom-DeflateString
�

PS ..\pwsh> 'ñ' | ConvertTo-DeflateString -Encoding utf8BOM | ConvertFrom-DeflateString
ñ
```

This example shows how different encodings affect the compression and decompression of special characters. The default encoding is `utf8NoBOM`.

### Example 4: Compress the contents of multiple files into a single Deflate Base64 string

```powershell
# Check the total length of the files
PS ..\pwsh> (Get-Content myLogs\*.txt | Measure-Object Length -Sum).Sum / 1kb
87.216796875

# Check the total length after compression
PS ..\pwsh> (Get-Content myLogs\*.txt | ConvertTo-DeflateString).Length / 1kb
35.123456789
```

This example demonstrates compressing the contents of multiple text files into a single Deflate-compressed Base64 string and compares the total length before and after compression.

## PARAMETERS

### -AsByteStream

Outputs the compressed byte array to the Success Stream.

> [!NOTE]
> This parameter is intended for use with cmdlets that accept byte arrays, such as `Out-File` and `Set-Content` with `-Encoding Byte` (Windows PowerShell 5.1) or `-AsByteStream` (PowerShell 7+).

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases: Raw

Required: False
Position: Named
Default value: False
Accept pipeline input: False
Accept wildcard characters: False
```

### -CompressionLevel

Specifies the compression level for the Deflate algorithm, balancing speed and compression size. See [`CompressionLevel` Enum](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.compressionlevel) for details.

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

### -Encoding

Determines the character encoding used when compressing the input strings.

> [!NOTE]
> The default encoding is UTF-8 without BOM.

```yaml
Type: Encoding
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: utf8NoBOM
Accept pipeline input: False
Accept wildcard characters: False
```

### -InputObject

Specifies the input string or strings to compress.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -NoNewLine

The encoded string representation of the input objects is concatenated to form the output. No newline character is added after each input string when this switch is used.

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

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

You can pipe one or more strings to this cmdlet.

## OUTPUTS

### System.String

By default, this cmdlet outputs a single Base64-encoded string.

### System.Byte[]

When the `-AsByteStream` switch is used, this cmdlet outputs a byte array down the pipeline.

## NOTES

## RELATED LINKS

[__`ConvertFrom-DeflateString`__](./ConvertFrom-DeflateString.md)

[__System.IO.Compression__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression)

[__DeflateStream Class__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.deflatestream)
