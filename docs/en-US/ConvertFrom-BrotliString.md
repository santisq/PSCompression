---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# ConvertFrom-BrotliString

## SYNOPSIS

Expands Brotli Base64 compressed input strings.

## SYNTAX

```powershell
ConvertFrom-BrotliString
    [-InputObject] <String[]>
    [-Encoding <Encoding>]
    [-Raw]
    [<CommonParameters>]
```

## DESCRIPTION

The `ConvertFrom-BrotliString` cmdlet expands Base64 encoded Brotli compressed strings using the `BrotliStream` class from the `BrotliSharpLib` library. This cmdlet is the counterpart of [`ConvertTo-BrotliString`](./ConvertTo-BrotliString.md).

## EXAMPLES

### Example 1: Expanding a Brotli compressed string

```powershell
PS ..\pwsh> ConvertFrom-BrotliString CwiAaGVsbG8NCndvcmxkDQohDQoD

hello
world
!
```

This example expands a Brotli Base64 encoded string back to its original strings.

### Example 2: Demonstrates how `-Raw` works

```powershell
PS ..\pwsh> $strings = 'hello', 'world', '!'

# New lines are preserved when the cmdlet receives an array of strings.
PS ..\pwsh> $strings | ConvertTo-BrotliString | ConvertFrom-BrotliString

hello
world
!

# When using the `-Raw` switch, all strings are returned as a single string
PS ..\pwsh> $strings | ConvertTo-BrotliString -NoNewLine | ConvertFrom-BrotliString -Raw

helloworld!
```

This example shows how the `-Raw` switch concatenates the expanded strings into a single string with newlines preserved.

## PARAMETERS

### -Encoding

Determines the character encoding used when expanding the input strings.

> [!NOTE]
> The default encoding is `utf8NoBOM`.

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

Outputs the expanded string as a single string with newlines preserved. By default, newline characters in the expanded string are used as delimiters to separate the input into an array of strings.

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

You can pipe Brotli Base64 strings to this cmdlet.

## OUTPUTS

### System.String

By default, this cmdlet streams strings. When the `-Raw` switch is used, it returns a single multi-line string.

## NOTES

## RELATED LINKS

[__ConvertTo-BrotliString__](https://github.com/santisq/PSCompression/)

[__BrotliSharpLib__](https://github.com/master131/BrotliSharpLib)

[__System.IO.Compression__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression)
