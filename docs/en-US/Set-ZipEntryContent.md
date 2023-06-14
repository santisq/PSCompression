---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Set-ZipEntryContent

## SYNOPSIS

Writes or appends content to an existing Zip Archive Entry.

## SYNTAX

### StringValue (Default)

```powershell
Set-ZipEntryContent -Value <Object[]> [-SourceEntry] <ZipEntryFile> [-Encoding <Encoding>] [-Append]
 [-PassThru] [<CommonParameters>]
```

### ByteStream

```powershell
Set-ZipEntryContent -Value <Object[]> [-SourceEntry] <ZipEntryFile> [-AsByteStream] [-Append]
 [-BufferSize <Int32>] [-PassThru] [<CommonParameters>]
```

## DESCRIPTION

The `Set-ZipEntryContent` cmdlet can write or append content to a Zip Archive Entry. By default, this cmdlet replaces the existing content of a Zip Archive Entry, if you need to append content you can use the `-Append` switch. This cmdlet also supports writing or appending raw bytes while using the `-AsByteStream` switch. To send content to `Set-ZipEntryContent` you can use the `-Value` parameter on the command line or send content through the pipeline.

If you need to create a new Zip Archive Entry you can use the [`New-ZipEntry` cmdlet](./New-ZipEntry.md).

## EXAMPLES

### Example 1

```powershell
PS ..pwsh\>
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
