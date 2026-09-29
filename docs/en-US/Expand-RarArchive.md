---
external help file: PSCompression.dll-Help.xml
Module Name: PSCompression
online version:
schema: 2.0.0
---

# Expand-RarArchive

## SYNOPSIS

Extracts files and directories from a RAR archive.

## SYNTAX

### Path

```powershell
Expand-RarArchive
    [-Path] <String[]>
    [[-Destination] <String>]
    [-Password <SecureString>]
    [-Force]
    [-PassThru]
    [<CommonParameters>]
```

### LiteralPath

```powershell
Expand-RarArchive
    [-Password <SecureString>]
    [[-Destination] <String>]
    [-Force]
    [-PassThru]
    -LiteralPath <String[]>
    [<CommonParameters>]
```

## DESCRIPTION

The `Expand-RarArchive` cmdlet extracts files and directories from RAR archives. It utilizes the `SharpCompress` library under the hood for archive decompression.

By default, contents are extracted to the current working location unless specified otherwise using the `-Destination` parameter. If an archive is password-protected and the `-Password` parameter is not supplied, the cmdlet automatically prompts the user for credentials. Use `-Force` to overwrite existing extracted files and `-PassThru` to output `FileInfo` and `DirectoryInfo` objects representing the extracted items.

## EXAMPLES

### Example 1: Extract a RAR archive to the current location

```powershell
PS /> Expand-RarArchive -Path .\archive.rar
PS /> Get-ChildItem

    Directory: C:\

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d----          2026-09-29  2:00 PM                folder1
-a---          2026-09-29  2:00 PM           1024 file1.txt
-a---          2026-09-29  2:00 PM           2048 file2.txt
```

This example extracts the contents of `archive.rar` to the current working directory, recreating the archived directory structure and files.

### Example 2: Extract an encrypted RAR archive to a specific destination

```powershell
PS /> $SecurePassword = Read-Host -AsSecureString
PS /> Expand-RarArchive -Path .\encrypted.rar -Destination .\output -Password $SecurePassword
```

This example extracts a password-protected RAR archive to the `.\output` directory using a pre-constructed `SecureString` password.

### Example 3: Pipeline input with PassThru

```powershell
PS /> Get-ChildItem *.rar | Expand-RarArchive -Destination .\extracted -PassThru

    Directory: C:\extracted

Mode                 LastWriteTime         Length Name
----                 -------------         ------ ----
d----          2026-09-29  2:00 PM                folder1
-a---          2026-09-29  2:00 PM           1024 file1.txt
-a---          2026-09-29  2:00 PM           2048 file2.txt
```

This example pipes multiple `.rar` files to `Expand-RarArchive` and uses `-PassThru` to return `FileInfo` and `DirectoryInfo` objects for all extracted files and directories.

## PARAMETERS

### -Destination

Specifies the path to the directory where the archive contents are extracted. If omitted, extraction defaults to the current working directory. The target directory is automatically created if it does not already exist.

```yaml
Type: String
Parameter Sets: (All)
Aliases:

Required: False
Position: 1
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Force

Overwrites existing files in the destination path without prompting. Without this switch, existing files will generate an error during extraction.

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

### -LiteralPath

Specifies the exact path(s) to the RAR archive(s) to extract. Unlike `-Path`, `-LiteralPath` does not evaluate wildcard characters and treats string input literally.

```yaml
Type: String[]
Parameter Sets: LiteralPath
Aliases: PSPath

Required: True
Position: Named
Default value: None
Accept pipeline input: True (ByPropertyName)
Accept wildcard characters: False
```

### -PassThru

Outputs `System.IO.FileInfo` and `System.IO.DirectoryInfo` decorated objects for each item extracted from the archive. By default, this cmdlet produces no output.

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

### -Password

Specifies a password as a `SecureString` for extracting encrypted RAR archives.

> [!NOTE]
> If the archive is encrypted and this parameter is omitted, the cmdlet prompts the host interface interactively for a password.

```yaml
Type: SecureString
Parameter Sets: (All)
Aliases:

Required: False
Position: Named
Default value: None
Accept pipeline input: False
Accept wildcard characters: False
```

### -Path

Specifies the path(s) to the RAR archive(s) to extract. Supports wildcard characters, allowing multiple archives to be targeted.

```yaml
Type: String[]
Parameter Sets: Path
Aliases:

Required: True
Position: 0
Default value: None
Accept pipeline input: True (ByValue)
Accept wildcard characters: True
```

### CommonParameters

This cmdlet supports the common parameters. For more information, see [about_CommonParameters](http://go.microsoft.com/fwlink/?LinkID=113216).

## INPUTS

### System.String[]

You can pipe paths to RAR archives to this cmdlet via the `-Path` or `-LiteralPath` parameters.

## OUTPUTS

### None

By default, this cmdlet returns no output.

### System.IO.FileInfo

### System.IO.DirectoryInfo

When `-PassThru` is specified, the cmdlet outputs FileInfo and DirectoryInfo objects representing extracted files and folders.

## NOTES

## RELATED LINKS

[__`Get-RarEntry`__](Get-RarEntry.md)

[__`Expand-RarEntry`__](Expand-RarEntry.md)

[__SharpCompress__](https://github.com/adamhathcock/sharpcompress)
