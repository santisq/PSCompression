---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Set-ZipEntryContent

## SYNOPSIS

{{ Fill in the Synopsis }}

## SYNTAX

### StringValue (Default)

```
Set-ZipEntryContent -Value <Object[]> [-SourceEntry] <ZipEntryFile> [-Encoding <Encoding>] [-Append]
 [-PassThru] [<CommonParameters>]
```

### ByteStream

```
Set-ZipEntryContent -Value <Object[]> [-SourceEntry] <ZipEntryFile> [-AsByteStream] [-Append]
 [-BufferSize <Int32>] [-PassThru] [<CommonParameters>]
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

### -Append

{{ Fill Append Description }}

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -AsByteStream

{{ Fill AsByteStream Description }}

```yaml
Type: SwitchParameter
Parameter Sets: ByteStream
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
Parameter Sets: ByteStream
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
Parameter Sets: StringValue
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -PassThru

{{ Fill PassThru Description }}

```yaml
Type: SwitchParameter
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -SourceEntry

{{ Fill SourceEntry Description }}

```yaml
Type: ZipEntryFile
Parameter Sets: (All)
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Value

{{ Fill Value Description }}

```yaml
Type: Object[]
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

### System.Object[]

## OUTPUTS

### PSCompression.ZipEntryFile

## NOTES

## RELATED LINKS
