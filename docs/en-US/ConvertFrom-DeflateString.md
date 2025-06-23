---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# ConvertFrom-DeflateString

## SYNOPSIS

Expands Deflate Base64 compressed input strings.

## SYNTAX

```powershell
ConvertFrom-DeflateString
    [-InputObject] <String[]>
    [-Encoding <Encoding>]
    [-Raw]
    [<CommonParameters>]
```

## DESCRIPTION

The `ConvertFrom-DeflateString` cmdlet expands Base64 encoded Deflate compressed strings using the [`DeflateStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.deflatestream). This cmdlet is the counterpart of [`ConvertTo-DeflateString`](./ConvertTo-DeflateString.md).

## EXAMPLES

### Example 1: Expanding a Deflate compressed string

```powershell
PS ..\pwsh> ConvertFrom-DeflateString ykjNycnn5SrPL8pJ4eVS5OUCAAAA//8DAA==

hello
world
!
```

This example expands a Deflate Base64 encoded string back to its original strings.

### Example 2: Demonstrates how `-Raw` works

```powershell
PS ..\pwsh> $strings = 'hello', 'world', '!'

# New lines are preserved when the cmdlet receives an array of strings.
PS ..\pwsh> $strings | ConvertTo-DeflateString | ConvertFrom-DeflateString

hello
world
!

# When using the `-Raw` switch, all strings are returned as a single string
PS ..\pwsh> $strings | ConvertTo-DeflateString -NoNewLine | ConvertFrom-DeflateString -Raw

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

You can pipe Deflate Base64 strings to this cmdlet.

## OUTPUTS

### System.String

By default, this cmdlet streams strings. When the `-Raw` switch is used, it returns a single multi-line string.

## NOTES

## RELATED LINKS

[__ConvertTo-DeflateString__](https://github.com/santisq/PSCompression)

[__System.IO.Compression__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression?view=net-6.0)

[__DeflateStream Class__](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.deflatestream)
