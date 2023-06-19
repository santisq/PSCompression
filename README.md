<div align="center">

# PSCompression

</div>

<div align="center">
    <sub>
        Zip and GZip utilities for PowerShell
    </sub>
    <br /><br />

[![build](https://github.com/santisq/PSCompression/actions/workflows/ci.yml/badge.svg)](https://github.com/santisq/PSCompression/actions/workflows/ci.yml)
[![codecov](https://codecov.io/gh/santisq/PSCompression/branch/main/graph/badge.svg?token=b51IOhpLfQ)](https://codecov.io/gh/santisq/PSCompression)
[![PSTree on PowerShell Gallery](https://img.shields.io/powershellgallery/v/PSCompression?label=gallery)](https://www.powershellgallery.com/packages/PSCompression)
[![LICENSE](https://img.shields.io/github/license/santisq/PSCompression)](https://github.com/santisq/PSCompression/blob/main/LICENSE)

</div>

PSCompression is a PowerShell Module aimed to provide Zip and Gzip utilities for compression, expansion and management as well as to solve a few issues with Zip compression existing in _built-in PowerShell_.

## Documentation

Check out [__the docs__](./docs/en-US/PSCompression.md) for information about how to use this Module.

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

Compatible with __Windows PowerShell 5.1__ and [__PowerShell 7+__](https://github.com/PowerShell/PowerShell).

## Contributing

Contributions are more than welcome, if you wish to contribute, fork this repository and submit a pull request with the changes.
