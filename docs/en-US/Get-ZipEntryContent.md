---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Get-ZipEntryContent

## SYNOPSIS
{{ Fill in the Synopsis }}

## SYNTAX

### Stream (Default)
```
Get-ZipEntryContent -ZipEntry <ZipEntryFile[]> [-Encoding <Encoding>] [-Stream] [<CommonParameters>]
```

### Raw
```
Get-ZipEntryContent -ZipEntry <ZipEntryFile[]> [-Encoding <Encoding>] [-Raw] [<CommonParameters>]
```

### Bytes
```
Get-ZipEntryContent -ZipEntry <ZipEntryFile[]> [-Stream] [-AsBytes] [-BufferSize <Int32>] [<CommonParameters>]
```

## DESCRIPTION
{{ Fill in the Description }}

## EXAMPLES

### Example 1
```powershell
PS C:\> {{ Add example code here }}
```

{{ Add example description here }}

## PARAMETERS

### -AsBytes
{{ Fill AsBytes Description }}

```yaml
Type: SwitchParameter
Parameter Sets: Bytes
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -BufferSize
{{ Fill BufferSize Description }}

```yaml
Type: Int32
Parameter Sets: Bytes
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Encoding
{{ Fill Encoding Description }}

```yaml
Type: Encoding
Parameter Sets: Stream, Raw
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Raw
{{ Fill Raw Description }}

```yaml
Type: SwitchParameter
Parameter Sets: Raw
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Stream
{{ Fill Stream Description }}

```yaml
Type: SwitchParameter
Parameter Sets: Stream, Bytes
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -ZipEntry
{{ Fill ZipEntry Description }}

```yaml
Type: ZipEntryFile[]
Parameter Sets: (All)
Aliases:

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: False
```

### CommonParameters
This cmdlet supports the common parameters: -Debug, -ErrorAction, -ErrorVariable, -InformationAction, -InformationVariable, -OutVariable, -OutBuffer, -PipelineVariable, -Verbose, -WarningAction, and -WarningVariable. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### PSCompression.ZipEntryFile[]

## OUTPUTS

### PSCompression.ZipEntryContent

## NOTES

## RELATED LINKS
