# CHANGELOG

## 06/17/2025

- Added commands supporting several algorithms to compress and decompress strings:
    - `ConvertFrom-BrotliString` & `ConvertTo-BrotliString` (using to BrotliSharpLib)
    - `ConvertFrom-DeflateString` & `ConvertTo-DeflateString` (from CLR)
    - `ConvertFrom-ZlibString` & `ConvertTo-ZlibString` (custom implementation)
- Added commands for `.tar` entry management with a reduced set of operations compared to `zip` entry management:
    - `Get-TarEntry`: Lists entries, serving as the main entry point for `TarEntry` cmdlets.
    - `Get-TarEntryContent`: Retrieves the content of a tar entry.
    - `Expand-TarEntry`: Extracts a tar entry to a file.
- Added commands to compress files and folders into `.tar` archives and extract `.tar` archives with various compression algorithms:
    - `Compress-TarArchive` and `Expand-TarArchive`: Supported compression algorithms include `gz`, `br`, `bz2`, `zst`, `lz`, and `none` (no compression).
- Removed commands:
    - `Compress-GzipArchive` and `Expand-GzipArchive`: These were deprecated as they only supported single-file compression, which is now better handled by the module’s `.tar` archive functionality.

This update was made possible by the following projects. If you find them helpful, please consider starring their repositories:

- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib)
- [SharpCompress](https://github.com/adamhathcock/sharpcompress)
- [BrotliSharpLib](https://github.com/master131/BrotliSharpLib)

## 01/10/2025

- Code improvements.
- Instance methods `.OpenRead()` and `.OpenWrite()` moved from `ZipEntryFile` to `ZipEntryBase`.
- Adds support to list, read and extract zip archive entries from Stream.

## 06/24/2024

- Update build process.

## 06/05/2024

- Update `ci.yml` to use `codecov-action@v4`.
- Fixed parameter names in `Compress-ZipArchive` documentation. Thanks to @martincostello.
- Fixed coverlet.console support for Linux runner tests.

## 02/26/2024

- Fixed a bug with `CompressionRatio` property showing always in InvariantCulture format.

## 02/25/2024

- `ZipEntryBase` Type:
    - Renamed Property `EntryName` to `Name`.
    - Renamed Property `EntryRelativePath` to `RelativePath`.
    - Renamed Property `EntryType` to `Type`.
    - Renamed Method `RemoveEntry()` to `Remove()`.
    - Added Property `CompressionRatio`.
- `ZipEntryFile` Type:
    - Added Property `Extension`.
    - Added Property `BaseName`.
- `ZipEntryDirectory` Type:
    - `.Name` Property now reflects the directory entries name instead of an empty string.
- Added command `Rename-ZipEntry`.
- `NormalizePath` Method:
    - Moved from `[PSCompression.ZipEntryExtensions]::NormalizePath` to `[PSCompression.Extensions.PathExtensions]::NormalizePath`.
- `Get-ZipEntry` command:
    - Renamed Parameter `-EntryType` to `-Type`.
