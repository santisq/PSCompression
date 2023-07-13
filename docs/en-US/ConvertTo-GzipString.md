---
external help file: PSCompression-help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# ConvertTo-GzipString

## SYNOPSIS

Creates a Gzip Base64 compressed string from a specified input string or strings.

## SYNTAX

```powershell
ConvertTo-GzipString [-InputObject] <String[]> [-Encoding <Encoding>] [-CompressionLevel <CompressionLevel>]
 [-AsByteStream] [-NoNewLine] [<CommonParameters>]
```

## DESCRIPTION

The `ConvertTo-GzipString` cmdlet can compress input strings into Gzip Base64 encoded strings or raw bytes using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream). For expansion of Base64 Gzip strings, see [`ConvertFrom-GzipString`](ConvertFrom-GzipString.md).

## EXAMPLES

### Example 1: Compress strings to Gzip compressed Base64 encoded string

```powershell
PS ..\pwsh> $strings = 'hello', 'world', '!'

# With positional binding
PS ..\pwsh> ConvertTo-GzipString $strings

H4sIAAAAAAAEAMtIzcnJ5+Uqzy/KSeHlUuTlAgBLr/K2EQAAAA==

# Or pipeline input, both work
PS ..\pwsh> $strings | ConvertTo-GzipString

H4sIAAAAAAAEAMtIzcnJ5+Uqzy/KSeHlUuTlAgBLr/K2EQAAAA==
```

### Example 2: Create a Gzip compressed file from a string

```powershell
PS ..\pwsh> 'hello world!' | ConvertTo-GzipString -AsByteStream |
    Compress-GzipArchive -DestinationPath .\files\file.gz
```

Demonstrates how `-AsByteStream` works on `ConvertTo-GzipString`, the cmdlet outputs a byte array that is received by `Compress-GzipArchive` and stored in a file. __Note that the byte array is not enumerated__.

### Example 3: Compress strings using a specific Encoding

```powershell
PS ..\pwsh> 'ñ' | ConvertTo-GzipString -Encoding ansi | ConvertFrom-GzipString
�

PS ..\pwsh> 'ñ' | ConvertTo-GzipString -Encoding utf8BOM | ConvertFrom-GzipString
ñ
```

The default Encoding is `utf8NoBom`.

### Example 4: Compressing multiple files into one Gzip Base64 string

```powershell
PS ..\pwsh> 0..10 | ForEach-Object {
    Invoke-RestMethod loripsum.net/api/10/long/plaintext -OutFile .\files\lorem$_.txt
}

# Check the total Length of the downloaded files
PS ..\pwsh> (Get-Content .\files\lorem*.txt | Measure-Object Length -Sum).Sum / 1kb
87.216796875

# Check the total Length after compression
PS ..\pwsh> (Get-Content .\files\lorem*.txt | ConvertTo-GzipString).Length / 1kb
36.94921875
```

## PARAMETERS

### -AsByteStream

Outputs the compressed byte array to the Success Stream.

> __NOTE:__ This parameter is meant to be used in combination with `Compress-GzipArchive`.

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

Define the compression level that should be used.
__See [`CompressionLevel` Enum](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.compressionlevel) for details__.

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

Determines the character encoding used when compressing the input strings. The default encoding is __`utf8NoBOM`__.

```yaml
Type: Encoding
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: Utf8
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

The encoded string representation of the input objects are concatenated to form the output.
No new line character is added after each output string when this switch is used.

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

### String

You can pipe strings to this cmdlet.

## OUTPUTS

### String

By default, this cmdlet outputs a single string.

### Byte[]

When the `-AsByteStream` switch is used this cmdlet outputs a byte array down the pipeline.
