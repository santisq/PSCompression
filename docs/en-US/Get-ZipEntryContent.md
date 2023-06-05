---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version: https://github.com/santisq/PSCompression
schema: 2.0.0
---

# Get-ZipEntryContent

## SYNOPSIS

Gets the content of a Zip Archive Entry.

## SYNTAX

### Stream (Default)

```powershell
Get-ZipEntryContent -ZipEntry <ZipEntryFile[]> [-Encoding <Encoding>] [-Raw] [<CommonParameters>]
```

### Bytes

```powershell
Get-ZipEntryContent -ZipEntry <ZipEntryFile[]> [-Raw] [-AsByteStream] [-BufferSize <Int32>] [<CommonParameters>]
```

## DESCRIPTION

The `Get-ZipEntryContent` cmdlet gets the content of one or more `ZipEntryFile` instances.
This cmdlet is meant to be used with `Get-ZipEntry` as your entry point.
The output entries from `Get-ZipEntry` cmdlet can be passed through the pipeline to this cmdlet.

## EXAMPLES

### Example 1: Get the content of a Zip Entry

```powershell
PS ..pwsh\> Get-ZipEntry .\myZip.zip -Include myrelative/entry.txt | Get-ZipEntryContent
```

`-Include` parameter from `Get-ZipEntry` can be used to target a specific entry by passing the entry's relative path, from there the output can be piped directly to `Get-ZipEntryContent`.
By default, the cmdlet streams line-by-line .

### Example 2: Get raw content of a Zip Entry

```powershell
PS ..pwsh\> Get-ZipEntry .\myZip.zip -Include myrelative/entry.txt | Get-ZipEntryContent -Raw
```

The cmdlet outputs a single multi-line string when the `-Raw` switch is used instead of line-by-line streaming.

### Example 3: Get the bytes of a Zip Entry

```powershell
PS ..pwsh\> $bytes = Get-ZipEntry .\test.zip -Include test/helloworld.txt | Get-ZipEntryContent -AsByteStream
PS ..pwsh\> $bytes
104
101
108
108
111
32
119
111
114
108
100
33
13
10

PS ..pwsh\> [System.Text.Encoding]::UTF8.GetString($bytes)
hello world!
```

The `-AsByteStream` switch can be useful to read non-text zip entries.

## PARAMETERS

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
Parameter Sets: Stream
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
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: False
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

### -AsByteStream

{{ Fill AsByteStream Description }}

```yaml
Type: SwitchParameter
Parameter Sets: Bytes
Aliases:

Required: False
Position: Named
Default value: False
Accept pipeline input: False
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
