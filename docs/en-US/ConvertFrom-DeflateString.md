---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# ConvertFrom-DeflateString

## SYNOPSIS

Decompresses Deflate-compressed Base64-encoded strings.

## SYNTAX

```powershell
ConvertFrom-DeflateString
    [-InputObject] <String[]>
    [-Encoding <Encoding>]
    [-Raw]
    [<CommonParameters>]
```

## DESCRIPTION

The ConvertFrom-DeflateString cmdlet decompresses Base64-encoded strings that were compressed using Deflate compression (via the [`DeflateStream` class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.deflatestream)). It is the counterpart to [`ConvertTo-DeflateString`](./ConvertTo-DeflateString.md).

## EXAMPLES

### Example 1: Decompress a Deflate-compressed Base64 string

```powershell
PS ..\pwsh> ConvertFrom-DeflateString ykjNycnn5SrPL8pJ4eVS5OUCAAAA//8DAA==

hello
world
!
```

This example decompresses a Deflate-compressed Base64 string, restoring the original multi-line text.

### Example 2: Compare default behavior with the `-Raw` switch

```powershell
PS ..\pwsh> $strings = 'hello', 'world', '!'
PS ..\pwsh> $compressed = $strings | ConvertTo-DeflateString
PS ..\pwsh> $decompressed = $compressed | ConvertFrom-DeflateString -Raw
PS ..\pwsh> $decompressed.GetType() # System.String
PS ..\pwsh> $decompressed

hello
world
!
```

This example compares the default behavior (outputting an array of strings split on newlines) with the `-Raw` switch (returning a single string with newlines preserved).

## PARAMETERS

### -Encoding

Specifies the text encoding to use for the decompressed output string(s).

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

Specifies the input string or strings to expand.

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

### -Raw

By default, the cmdlet splits the decompressed text on newline characters and outputs an array of strings. The `-Raw` switch returns the entire decompressed text as a single string (with newlines preserved).

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

You can pipe Deflate-compressed Base64 strings to this cmdlet.

## OUTPUTS

### System.String

By default: `System.String[]` (one element per line). With `-Raw`: `System.String` (single multi-line string).

## NOTES

## RELATED LINKS

[__`ConvertTo-DeflateString`__](./ConvertTo-DeflateString.md)

[__System.IO.Compression__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression)

[__DeflateStream Class__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.deflatestream)
