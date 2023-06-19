---
Module Name: PSCompression
Module Guid: c63aa90e-ae64-4ae1-b1c8-456e0d13967e
Download Help Link:
Help Version: 1.0.0.0
Locale: en-US
---

# PSCompression Module

## Description

PSCompression is a PowerShell Module aimed to provide Zip and Gzip utilities for compression, expansion and management as well as to solve a few issues with Zip compression existing in _built-in PowerShell_.

## PSCompression Cmdlets

### Zip Cmdlets

#### [Expand-ZipEntry](Expand-ZipEntry.md)

The `Expand-ZipEntry` cmdlet can expand Zip Archive Entries outputted by the [`Get-ZipEntry`](Get-ZipEntry.md) command to a destination directory.

#### [Get-ZipEntry](Get-ZipEntry.md)

The `Get-ZipEntry` cmdlet lists entries from specified Zip paths. It has built-in functionalities to filter entries and is the main entry point for the ZipEntry cmdlets in this module.

#### [Get-ZipEntryContent](Get-ZipEntryContent.md)

The `Get-ZipEntryContent` cmdlet gets the content of one or more `ZipEntryFile` instances.

#### [New-ZipEntry](New-ZipEntry.md)

The `New-ZipEntry` cmdlet can create one or more Zip Archive Entries from specified paths.

#### [Remove-ZipEntry](Remove-ZipEntry.md)

The `Remove-ZipEntry` cmdlet can remove Zip Archive Entries from one or more Zip Archives.

#### [Set-ZipEntryContent](Set-ZipEntryContent.md)

The `Set-ZipEntryContent` cmdlet can write or append content to a Zip Archive Entry.

#### [Compress-ZipArchive](Compress-ZipArchive.md)

The `Compress-ZipArchive` cmdlet creates a compressed, or zipped, archive file from one or more specified files or directories. It aims to overcome a few limitations of [`Compress-Archive`](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.archive/compress-archive?view=powershell-7.2) while keeping similar pipeline capabilities.

### Gzip Cmdlets

#### [Compress-GzipArchive](Compress-GzipArchive.md)

The `Compress-GzipArchive` cmdlet can compress one or more specified file paths into a single Gzip file.

#### [ConvertFrom-GzipString](ConvertFrom-GzipString.md)

The `ConvertFrom-GzipString` cmdlet aims to expand Base64 encoded Gzip compressed input strings.

#### [ConvertTo-GzipString](ConvertTo-GzipString.md)

The `ConvertTo-GzipString` cmdlet aims to compress input strings into Base64 encoded Gzip strings or raw bytes.

#### [Expand-GzipArchive](Expand-GzipArchive.md)

The `Expand-GzipArchive` cmdlet aims to expand Gzip compressed files to a destination path or to the [success stream](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_output_streams?view=powershell-7.3#success-stream).
