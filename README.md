<h1 align="center">PSCompression</h1>

<div align="center">
    <sub>
        Zip and GZip utilities for PowerShell
    </sub>
    <br /><br />

[![build](https://github.com/santisq/PSCompression/actions/workflows/ci.yml/badge.svg)](https://github.com/santisq/PSCompression/actions/workflows/ci.yml)
[![PSTree on PowerShell Gallery](https://img.shields.io/powershellgallery/v/PSCompression?label=gallery)](https://www.powershellgallery.com/packages/PSCompression)
[![LICENSE](https://img.shields.io/github/license/santisq/PSCompression)](https://github.com/santisq/PSCompression/blob/main/LICENSE)

</div>

PSCompression is a PowerShell Module aimed to provide Gzip utilities for compression and expansion as well as to solve a few issues with Zip compression existing in _built-in PowerShell_.

## Installation

The module is available through the [PowerShell Gallery](https://www.powershellgallery.com/):

```powershell
Install-Module PSCompression -Scope CurrentUser
```

## Documentation

### [`Compress-ZipArchive`](/docs/Compress-ZipArchive.md)

PowerShell function that overcomes the limitations of [`Compress-Archive`](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.archive/compress-archive?view=powershell-7.2) while keeping similar pipeline capabilities.

### [`Compress-GzipArchive`](/docs/Compress-GzipArchive.md)

PowerShell function aimed to compress multiple files into a single Gzip compressed file using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream).

### [`Expand-GzipArchive`](/docs/Expand-GzipArchive.md)

PowerShell function aimed to expand Gzip files using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream).

### [`ConvertTo-GzipString`](/docs/ConvertTo-GzipString.md)

PowerShell function aimed to compress input strings into Base64 Gzip compressed strings or raw bytes using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream).

### [`ConvertFrom-GzipString`](/docs/ConvertFrom-GzipString.md)

PowerShell function aimed to expand Base64 Gzip compressed input strings using the [`GzipStream` Class](https://learn.microsoft.com/en-us/dotnet/api/system.io.compression.gzipstream).

## Contributing

Contributions are more than welcome, if you wish to contribute, fork this repository and submit a pull request with the changes.
