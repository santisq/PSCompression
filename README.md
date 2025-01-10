<h1 align="center">PSCompression</h1>
<div align="center">
<sub>Zip and GZip utilities for PowerShell</sub>
<br/><br/>

[![build](https://github.com/santisq/PSCompression/actions/workflows/ci.yml/badge.svg)](https://github.com/santisq/PSCompression/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/santisq/PSCompression/branch/main/graph/badge.svg)](https://codecov.io/gh/santisq/PSCompression)
[![PowerShell Gallery](https://img.shields.io/powershellgallery/dt/PSCompression?color=%23008FC7
)](https://www.powershellgallery.com/packages/PSCompression)
[![LICENSE](https://img.shields.io/github/license/santisq/PSCompression)](https://github.com/santisq/PSCompression/blob/main/LICENSE)

</div>

PSCompression is a PowerShell Module that provides Zip and Gzip utilities for compression, expansion and management. It also solves a few issues with Zip compression existing in _built-in PowerShell_.

## What does this Module offer?

### Zip Cmdlets

<div class="zipcmdlets">
<table>
<tr>
<th>Cmdlet</th>
<th>Description</th>
</tr>
<tr>
<td colspan="1" width="230" height="60">

[`Get-ZipEntry`](docs/en-US/Get-ZipEntry.md)

</td>
<td>

Main entry point for the `*-ZipEntry` cmdlets in this module. It can list zip archive entries from specified paths or input stream.

</td>
</tr>
<tr>
<td colspan="1" width="230" height="60">

[`Expand-ZipEntry`](docs/en-US/Expand-ZipEntry.md)

</td>
<td>

Expands zip entries to a destination directory.

</td>
</tr>
<tr>
<td colspan="1" width="230" height="60">

[`Get-ZipEntryContent`](docs/en-US/Get-ZipEntryContent.md)

</td>
<td>

Gets the content of one or more zip entries.

</td>
</tr>
<tr>
<td colspan="1" width="230" height="60">

[`New-ZipEntry`](docs/en-US/New-ZipEntry.md)

</td>
<td>Creates zip entries from specified path or paths.</td>
</tr>
<tr>
<td colspan="1" width="230" height="60">

[`Remove-ZipEntry`](docs/en-US/Remove-ZipEntry.md)

</td>
<td>Removes zip entries from one or more zip archives.</td>
</tr>
<tr>
<td colspan="1" width="230" height="60">

[`Rename-ZipEntry`](docs/en-US/Rename-ZipEntry.md)

</td>
<td>Renames zip entries from one or more zip archives.</td>
</tr>
<tr>
<td colspan="1" width="230" height="60">

[`Set-ZipEntryContent`](docs/en-US/Set-ZipEntryContent.md)

</td>
<td>Sets or appends content to a zip entry.</td>
</tr>
<tr>
<td colspan="1" width="230" height="60">

[`Compress-ZipArchive`](docs/en-US/Compress-ZipArchive.md)

</td>
<td>

Similar capabilities as
[`Compress-Archive`](docs/en-US/https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.archive/compress-archive?view=powershell-7.2)
and overcomes a few issues with the built-in cmdlet (2 GB limit and more).

</td>
</tr>
</table>
</div>

### Gzip Cmdlets

<div class="gzipcmdlets">
<table>
<tr>
<th>Cmdlet</th>
<th>Description</th>
</tr>
<tr>
<td colspan="1" width="230" height="60">

[`Compress-GzipArchive`](docs/en-US/Compress-GzipArchive.md)

</td>
<td>
Can compress one or more specified file paths into a Gzip file.
</td>
</tr>
<tr>
<td colspan="1" width="230" height="60">

[`ConvertFrom-GzipString`](docs/en-US/ConvertFrom-GzipString.md)

</td>
<td>
Expands Gzip Base64 input strings.
</td>
</tr>

<tr>
<td colspan="1" width="230" height="60">

[`ConvertTo-GzipString`](docs/en-US/ConvertTo-GzipString.md)

</td>
<td>
Can compress input strings into Gzip Base64 strings or raw bytes.
</td>
</tr>

<tr>
<td colspan="1" width="230" height="60">

[`Expand-GzipArchive`](docs/en-US/Expand-GzipArchive.md)

</td>
<td>

Expands Gzip compressed files to a destination path or to the [success stream](https://learn.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_output_streams?view=powershell-7.3#success-stream).

</td>
</tr>
</table>
</div>

## Documentation

Check out [__the docs__](docs/en-US) for information about how to use this Module.

## Installation

### Gallery

The module is available through the [PowerShell Gallery](https://www.powershellgallery.com/):

```powershell
Install-Module PSCompression -Scope CurrentUser
```

### Source

```powershell
git clone 'https://github.com/santisq/PSCompression.git'
Set-Location ./PSCompression
./build.ps1
```

## Requirements

This module has no external requirements and is compatible with __Windows PowerShell 5.1__ and [__PowerShell 7+__](https://github.com/PowerShell/PowerShell).

## Contributing

Contributions are more than welcome, if you wish to contribute, fork this repository and submit a pull request with the changes.
