---
external help file: PSCompression-help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# ConvertTo-GzipString

## SYNOPSIS
Creates a Base64 encoded Gzip compressed string from a specified input string or strings.

## SYNTAX

```powershell
ConvertTo-GzipString [-InputObject] <String[]> [[-Encoding] <Encoding>]
 [[-CompressionLevel] <CompressionLevel>] [-Raw] [-NoNewLine] [<CommonParameters>]
```

## DESCRIPTION
PowerShell function aimed to compress input strings into Base64 encoded Gzip strings or raw bytes using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream). For expansion of Base64 Gzip strings, see [`ConvertFrom-GzipString`](/docs/ConvertFrom-GzipString.md).

## EXAMPLES
__All Gzip Examples can be found [here](/docs/GzipExamples.md).__

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
Default value: Utf-8
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

### -Raw
Outputs the compressed bytes to the Success Stream.
There is no Base64 Encoding when this parameter is used.
This parameter is meant to be used in combination with [`Compress-GzipArchive`](/docs/Compress-GzipArchive.md).

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
The encoded string representations of the input objects are concatenated to form the output.
No spaces or newlines are inserted between the output strings.
No newline is added after the last output string.

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
