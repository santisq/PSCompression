# CHANGELOG

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
