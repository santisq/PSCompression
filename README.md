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

__New in recent updates:__ Full support for reading and extracting __password-protected (encrypted) zip entries__, including AES encryption.

## Features

- __Zip Archive Management__: Create, list, extract, retrieve content, modify, and remove entries in zip archives with pipeline support.  
  Now includes __full support for password-protected entries__ (traditional ZipCrypto and AES encryption) via `-Password` on `Get-ZipEntryContent` and `Expand-ZipEntry`. Zip entries also expose native properties like `IsEncrypted`, `AESKeySize`, `CompressionMethod`, `Comment`, and `Crc`.
- __Tar Archive Management__: Compress and extract tar archives with support for `gz`, `bz2`, `zst`, `lz`, and uncompressed (`none`) formats.
- __Tar Entry Management__: List, extract, and retrieve content from individual tar entries.
- __String Compression__: Compress and decompress strings using Brotli, Deflate, Gzip, and Zlib algorithms.

## Cmdlets

### Zip Archive

- [__`Compress-ZipArchive`__](docs/en-US/Compress-ZipArchive.md) — Compresses files and folders into a zip archive, overcoming built-in PowerShell limitations.
- [__`Expand-ZipEntry`__](docs/en-US/Expand-ZipEntry.md) — Extracts individual zip entries to a destination directory.
- [__`Get-ZipEntry`__](docs/en-US/Get-ZipEntry.md) — Lists zip archive entries from paths or streams, serving as the entry point for zip cmdlets.
- [__`Get-ZipEntryContent`__](docs/en-US/Get-ZipEntryContent.md) — Retrieves the content of zip entries as text or bytes.
- [__`New-ZipEntry`__](docs/en-US/New-ZipEntry.md) — Adds new entries to a zip archive from files or paths.
- [__`Remove-ZipEntry`__](docs/en-US/Remove-ZipEntry.md) — Removes entries from one or more zip archives.
- [__`Rename-ZipEntry`__](docs/en-US/Rename-ZipEntry.md) — Renames entries in one or more zip archives.
- [__`Set-ZipEntryContent`__](docs/en-US/Set-ZipEntryContent.md) — Sets or appends content to a zip entry.

> [!NOTE]
> Due to a .NET limitation, cmdlets like `New-ZipEntry`, `Compress-ZipArchive` with `-Update`, and `Set-ZipEntryContent` may fail when handling files or content > 2 GB __in existing zip archives__. As a workaround, recreate the zip archive or use tools like 7-Zip, which support larger files. See [issue #19](https://github.com/santisq/PSCompression/issues/19) for details.

### Tar Archive

- [__`Compress-TarArchive`__](docs/en-US/Compress-TarArchive.md) — Compresses files and folders into a tar archive with optional compression (gz, bz2, zst, lz, none).
- [__`Expand-TarArchive`__](docs/en-US/Expand-TarArchive.md) — Extracts a tar archive with support for gz, bz2, zst, lz, and uncompressed formats.
- [__`Expand-TarEntry`__](docs/en-US/Expand-TarEntry.md) — Extracts individual tar entries to a destination directory.
- [__`Get-TarEntry`__](docs/en-US/Get-TarEntry.md) — Lists tar archive entries from paths or streams.
- [__`Get-TarEntryContent`__](docs/en-US/Get-TarEntryContent.md) — Retrieves the content of tar entries as text or bytes.

### String Compression

- [__`ConvertFrom-BrotliString`__](docs/en-US/ConvertFrom-BrotliString.md) — Decompresses a Brotli-compressed string.
- [__`ConvertFrom-DeflateString`__](docs/en-US/ConvertFrom-DeflateString.md) — Decompresses a Deflate-compressed string.
- [__`ConvertFrom-GzipString`__](docs/en-US/ConvertFrom-GzipString.md) — Decompresses a Gzip-compressed string.
- [__`ConvertFrom-ZlibString`__](docs/en-US/ConvertFrom-ZlibString.md) — Decompresses a Zlib-compressed string.
- [__`ConvertTo-BrotliString`__](docs/en-US/ConvertTo-BrotliString.md) — Compresses a string using Brotli.
- [__`ConvertTo-DeflateString`__](docs/en-US/ConvertTo-DeflateString.md) — Compresses a string using Deflate.
- [__`ConvertTo-GzipString`__](docs/en-US/ConvertTo-GzipString.md) — Compresses a string using Gzip.
- [__`ConvertTo-ZlibString`__](docs/en-US/ConvertTo-ZlibString.md) — Compresses a string using Zlib.

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
