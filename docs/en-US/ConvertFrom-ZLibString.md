---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# ConvertFrom-ZLibString

## SYNOPSIS

Decompresses ZLib-compressed Base64-encoded strings.

## SYNTAX

```powershell
ConvertFrom-ZLibString
    [-InputObject] <String[]>
    [-Encoding <Encoding>]
    [-Raw]
    [<CommonParameters>]
```

## DESCRIPTION

The `ConvertFrom-ZLibString` cmdlet decompresses Base64-encoded strings that were compressed using ZLib compression. It uses a custom ZLib implementation built on top of the [`DeflateStream` class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.deflatestream). For implementation details, see the PSCompression source code.
This cmdlet is the counterpart of [`ConvertTo-ZLibString`](./ConvertTo-ZLibString.md)."

## EXAMPLES

### Example 1: Decompress a ZLib-compressed Base64 string

```powershell
PS ..\pwsh> ConvertFrom-ZLibString eJzKSM3JyeflKs8vyknh5VLk5QIAAAD//wMAMosEow==

hello
world
!
```

This example decompresses a ZLib-compressed Base64 string, restoring the original multi-line text.

### Example 2: Compare default behavior with the `-Raw` switch

```powershell
PS ..\pwsh> $strings = 'hello', 'world', '!'
PS ..\pwsh> $compressed = $strings | ConvertTo-ZlibString
PS ..\pwsh> $decompressed = $compressed | ConvertFrom-ZlibString -Raw
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

Specifies the ZLib-compressed Base64 string(s) to decompress.

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

You can pipe ZLib-compressed Base64 strings to this cmdlet.

## OUTPUTS

### System.String

By default: `System.String[]` (one element per line). With `-Raw`: `System.String` (single multi-line string).

## NOTES

## RELATED LINKS

[__`ConvertTo-ZLibString`__](./ConvertTo-ZLibString.md)

[__System.IO.Compression__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression)

[__DeflateStream Class__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.deflatestream)

[__ZlibStream Class__](https://github.com/santisq/PSCompression/blob/main/src/PSCompression/ZlibStream.cs)
