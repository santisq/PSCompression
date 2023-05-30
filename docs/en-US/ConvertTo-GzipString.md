---
external help file: PSCompression-help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# ConvertTo-GzipString

## SYNOPSIS

Creates a Base64 Gzip compressed string from a specified input string or strings.

## SYNTAX

```powershell
ConvertTo-GzipString [-InputObject] <String[]> [[-Encoding] <Encoding>]
 [[-CompressionLevel] <CompressionLevel>] [-AsByteStream] [-NoNewLine] [<CommonParameters>]
```

## DESCRIPTION

PowerShell cmdlet aimed to compress input strings into Base64 encoded Gzip strings or raw bytes using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream). For expansion of Base64 Gzip strings, see `ConvertFrom-GzipString`.

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
# Demonstrates how `-AsByteStream` works on `ConvertTo-GzipString`.
# Sends the compressed bytes to `Compress-GzipArchive`.
PS ..\pwsh> 'hello world!' | ConvertTo-GzipString -AsByteStream |
    Compress-GzipArchive -DestinationPath .\files\file.gzip
```

### Example 3: Compress strings using a specific Encoding

```powershell
# Note: Default encoding is utf8NoBom
PS ..\pwsh> 'ñ' | ConvertTo-GzipString -Encoding ansi | ConvertFrom-GzipString
�

PS ..\pwsh> 'ñ' | ConvertTo-GzipString -Encoding utf8BOM | ConvertFrom-GzipString
ñ
```

### Example 4: Compressing multiple files into one Gzip Base64 string

```powershell
PS ..\pwsh> 0..10 | ForEach-Object {
    Invoke-RestMethod loripsum.net/api/10/long/plaintext -OutFile .\files\lorem$_.txt
}

PS ..\pwsh> (Get-Content .\files\lorem*.txt | ConvertTo-GzipString).Length / 1kb

36.94921875

PS ..\pwsh> (Get-Content .\files\lorem*.txt | ConvertTo-GzipString | ConvertFrom-GzipString -Raw).Length / 1kb

87.216796875
```

## PARAMETERS

### -InputObject

Specifies the input string or strings to compress.

```yaml
Type: String[]
Parameter Sets: (All)
Aliases:

Required: True
Position: 1
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### -Encoding

Character encoding used when compressing the Gzip input.

```yaml
Type: Encoding
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: utf8
Accept pipeline input: False
Accept wildcard characters: False
```

### -CompressionLevel

Define the compression level that should be used.
See [`CompressionLevel` Enum](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.compressionlevel) for details.

```yaml
Type: CompressionLevel
Parameter Sets: (All)
Aliases:
Accepted values: Optimal, Fastest, NoCompression, SmallestSize

Required: False
Position: 3
Default value: Optimal
Accept pipeline input: False
Accept wildcard characters: False
```

### -AsByteStream

Outputs the compressed bytes to the Success Stream.
There is no Base64 Encoding when this parameter is used.
This parameter is meant to be used in combination with `Compress-GzipArchive`.

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

### -NoNewLine

The encoded string representation of the input objects are concatenated to form the output.
No new line character is added after the last output string when this switch is used.

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

## OUTPUTS

### String

By default, this cmdlet outputs a single string.

### Byte

When the `-AsByteStream` switch is used this cmdlet outputs a stream of bytes down the pipeline.
