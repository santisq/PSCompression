# 02/26/2024

- Fixed a bug with `CompressionRatio` property showing always in InvariantCulture format.

# 02/25/2024

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
