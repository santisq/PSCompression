---
external help file: PSCompression-help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# ConvertFrom-GzipString

## SYNOPSIS
Expands a Base64 encoded Gzip compressed input strings.

## SYNTAX

```powershell
ConvertFrom-GzipString [-InputObject] <String[]> [[-Encoding] <Encoding>] [-Raw] [<CommonParameters>]
```

## DESCRIPTION
PowerShell function aimed to expand Base64 encoded Gzip compressed input strings using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream). This function is the counterpart of [`ConvertTo-GzipString`](/docs/ConvertTo-GzipString.md).

## EXAMPLES

__All Gzip Examples can be found [here](/docs/GzipExamples.md).__

## PARAMETERS

### -InputObject
The Base64 encoded Gzip compressed string or strings to expand.

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
Character encoding used when expanding the Gzip strings.

```yaml
Type: Encoding
Parameter Sets: (All)
Aliases:

Required: False
Position: 2
Default value: Utf8
Accept pipeline input: False
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
