---
external help file: PSCompression-help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# ConvertFrom-GzipString

## SYNOPSIS

Expands Base64 Gzip compressed input strings.

## SYNTAX

```powershell
ConvertFrom-GzipString [-InputObject] <String[]> [[-Encoding] <Encoding>] [-Raw] [<CommonParameters>]
```

## DESCRIPTION

The `ConvertFrom-GzipString` cmdlet can expand Base64 encoded Gzip compressed strings using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream). This cmdlet is the counterpart of [`ConvertTo-GzipString`](ConvertTo-GzipString.md).

## EXAMPLES

### Example 1: Expanding a Gzip compressed string

```powershell
PS ..\pwsh> ConvertFrom-GzipString H4sIAAAAAAAACstIzcnJ5+Uqzy/KSeHlUuTlAgBLr/K2EQAAAA==

hello
world
!
```

### Example 2: Demonstrates how `-NoNewLine` works

```powershell
PS ..\pwsh> $strings = 'hello', 'world', '!'

# New lines are preserved when the cmdlet receives an array of strings.
PS ..\pwsh> $strings | ConvertTo-GzipString | ConvertFrom-GzipString

hello
world
!

# When using the `-NoNewLine` switch, all strings are concatenated
PS ..\pwsh> $strings | ConvertTo-GzipString -NoNewLine | ConvertFrom-GzipString

helloworld!
```

## PARAMETERS

### -Encoding

Determines the character encoding used when expanding the input strings. The default encoding is __`utf8NoBOM`__.

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

Outputs the expanded string as a single string with newlines preserved.
By default, newline characters in the expanded string are used as delimiters to separate the input into an array of strings.

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

You can pipe Gzip Base64 strings to this cmdlet.

## OUTPUTS

### String

By default, this cmdlet streams strings. When the `-Raw` switch is used, it returns a single multi-line string.
