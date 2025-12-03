<h1 align="center">PSCompression</h1>
<div align="center">
<sub>Zip, tar, and string compression utilities for PowerShell!</sub>
<br/><br/>

[![build](https://github.com/santisq/PSCompression/actions/workflows/ci.yml/badge.svg)](https://github.com/santisq/PSCompression/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/santisq/PSCompression/branch/main/graph/badge.svg)](https://codecov.io/gh/santisq/PSCompression)
[![PowerShell Gallery](https://img.shields.io/powershellgallery/dt/PSCompression?color=%23008FC7
)](https://www.powershellgallery.com/packages/PSCompression)
[![LICENSE](https://img.shields.io/github/license/santisq/PSCompression)](https://github.com/santisq/PSCompression/blob/main/LICENSE)

</div>

`PSCompression` is a PowerShell module that provides utilities for creating, managing, and extracting zip and tar archives, as well as compressing and decompressing strings. It overcomes limitations in built-in PowerShell archive cmdlets (e.g., 2 GB zip file limits) and supports multiple compression algorithms, including gzip, bzip2, Zstandard, lzip, Brotli, Deflate, and Zlib. Built for cross-platform use, it’s compatible with Windows, Linux, and macOS.

## Features

- __Zip Archive Management__: Create, list, extract, retrieve content, modify, and remove entries in zip archives with pipeline support.
- __Tar Archive Management__: Compress and extract tar archives with support for `gz`, `bz2`, `zst`, `lz`, and uncompressed (`none`) formats.
- __Tar Entry Management__: List, extract, and retrieve content from individual tar entries.
- __String Compression__: Compress and decompress strings using Brotli, Deflate, Gzip, and Zlib algorithms.

## Cmdlets

### Zip Archive Cmdlets

<table>
  <tr>
    <th>Cmdlet</th>
    <th>Description</th>
  </tr>
  <tr>
    <td width="220" height="70"><a href="docs/en-US/Compress-ZipArchive.md"><code>Compress-ZipArchive</code></a></td>
    <td>Compresses files and folders into a zip archive, overcoming built-in PowerShell limitations.</td>
  </tr>
  <tr>
    <td width="220" height="70"><a href="docs/en-US/Expand-ZipEntry.md"><code>Expand-ZipEntry</code></a></td>
    <td>Extracts individual zip entries to a destination directory.</td>
  </tr>
  <tr>
    <td width="220" height="70"><a href="docs/en-US/Get-ZipEntry.md"><code>Get-ZipEntry</code></a></td>
    <td>Lists zip archive entries from paths or streams, serving as the entry point for zip cmdlets.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/Get-ZipEntryContent.md"><code>Get-ZipEntryContent</code></a></td>
    <td>Retrieves the content of zip entries as text or bytes.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/New-ZipEntry.md"><code>New-ZipEntry</code></a></td>
    <td>Adds new entries to a zip archive from files or paths.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/Remove-ZipEntry.md"><code>Remove-ZipEntry</code></a></td>
    <td>Removes entries from one or more zip archives.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/Rename-ZipEntry.md"><code>Rename-ZipEntry</code></a></td>
    <td>Renames entries in one or more zip archives.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/Set-ZipEntryContent.md"><code>Set-ZipEntryContent</code></a></td>
    <td>Sets or appends content to a zip entry.</td>
  </tr>
</table>

> [!NOTE]
> Due to a .NET limitation, cmdlets like `New-ZipEntry`, `Compress-ZipArchive` with `-Update`, and `Set-ZipEntryContent` may fail when handling files or content > 2 GB __in existing zip archives__. As a workaround, recreate the zip archive or use tools like 7-Zip, which support larger files. See [issue #19](https://github.com/santisq/PSCompression/issues/19) for details.

### Tar Archive Cmdlets

<table>
  <tr>
    <th>Cmdlet</th>
    <th>Alias</th>
    <th>Description</th>
  </tr>
  <tr>
    <td colspan="1" width="220"><a href="docs/en-US/Compress-TarArchive.md">Compress-TarArchive</a></td>
    <td><code>tarcompress</code></td>
    <td>Compresses files and folders into a tar archive with optional compression (gz, bz2, zst, lz, none).</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/Expand-TarArchive.md">Expand-TarArchive</a></td>
    <td><code>untar</code></td>
    <td>Extracts a tar archive with support for gz, bz2, zst, lz, and uncompressed formats.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/Expand-TarEntry.md">Expand-TarEntry</a></td>
    <td><code>untarentry</code></td>
    <td>Extracts individual tar entries to a destination directory.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/Get-TarEntry.md">Get-TarEntry</a></td>
    <td><code>targe</code></td>
    <td>Lists tar archive entries from paths or streams, serving as the entry point for tar cmdlets.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/Get-TarEntryContent.md">Get-TarEntryContent</a></td>
    <td><code>targec</code></td>
    <td>Retrieves the content of tar entries as text or bytes.</td>
  </tr>
</table>

### String Compression Cmdlets

<table>
  <tr>
    <th>Cmdlet</th>
    <th>Alias</th>
    <th>Description</th>
  </tr>
  <tr>
    <td colspan="1" width="220"><a href="docs/en-US/ConvertFrom-BrotliString.md">ConvertFrom-BrotliString</a></td>
    <td><code>frombrotlistring</code></td>
    <td>Decompresses a Brotli-compressed string.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/ConvertFrom-DeflateString.md">ConvertFrom-DeflateString</a></td>
    <td><code>fromdeflatestring</code></td>
    <td>Decompresses a Deflate-compressed string.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/ConvertFrom-GzipString">ConvertFrom-GzipString</a></td>
    <td><code>fromgzipstring</code></td>
    <td>Decompresses a Gzip-compressed string.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/ConvertFrom-ZlibString.md">ConvertFrom-ZlibString</a></td>
    <td><code>fromzlibstring</code></td>
    <td>Decompresses a Zlib-compressed string.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/ConvertTo-BrotliString.md">ConvertTo-BrotliString</a></td>
    <td><code>tobrotlistring</code></td>
    <td>Compresses a string using Brotli.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/ConvertTo-DeflateString.md">ConvertTo-DeflateString</a></td>
    <td><code>todeflatestring</code></td>
    <td>Compresses a string using Deflate.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/ConvertTo-GzipString.md">ConvertTo-GzipString</a></td>
    <td><code>togzipstring</code></td>
    <td>Compresses a string using Gzip.</td>
  </tr>
  <tr>
    <td><a href="docs/en-US/ConvertTo-ZlibString.md">ConvertTo-ZlibString</a></td>
    <td><code>tozlibstring</code></td>
    <td>Compresses a string using Zlib.</td>
  </tr>
</table>

> [!NOTE]
> The `Compress-GzipArchive` and `Expand-GzipArchive` cmdlets have been removed, as their single-file gzip functionality is now handled by `Compress-TarArchive` and `Expand-TarArchive`. For a workaround to compress or decompress single files using gzip, see [Example 2 in `ConvertTo-GzipString`](./docs/en-US/ConvertTo-GzipString.md#example-2-create-a-gzip-compressed-file-from-a-string).

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

## Acknowledgments

This module is powered by the following open-source projects:

- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib)
- [SharpCompress](https://github.com/adamhathcock/sharpcompress)
- [BrotliSharpLib](https://github.com/master131/BrotliSharpLib)
- [ZstdSharp](https://github.com/oleg-st/ZstdSharp)
- [System.IO.Compression](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression)

If you find these projects helpful, consider starring their repositories!

## Contributing

Contributions are more than welcome, if you wish to contribute, fork this repository and submit a pull request with the changes.
