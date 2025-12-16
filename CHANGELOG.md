# CHANGELOG

## 12/13/2025

- **Native Zip Entry Objects**

  Zip entries returned by `Get-ZipEntry` (and created by `New-ZipEntry`) are now backed directly by `ICSharpCode.SharpZipLib.Zip.ZipEntry`.  
  This exposes additional useful properties on `ZipEntryBase` derived objects:
  - `IsEncrypted` (`bool`) – Indicates whether the entry is encrypted.
  - `AESKeySize` (`int`) – AES key size (0, 128, 192, or 256) if AES encryption is used.
  - `CompressionMethod` (`ICSharpCode.SharpZipLib.Zip.CompressionMethod`) – The actual compression method used.
  - `Comment` (`string`) – The entry comment.
  - `Crc` (`long`) – Cyclic redundancy check.

- **Support for Encrypted Zip Entries**  

  `Get-ZipEntryContent` and `Expand-ZipEntry` now fully support reading and extracting password-protected entries.  
  A new common parameter has been added to both cmdlets:

  ```powershell
  -Password <SecureString>
  ```

  - If an entry is encrypted and no password is provided, the cmdlets will securely prompt for one.
  - Examples and detailed guidance for handling encrypted entries have been added to the help documentation.
  
- **Documentation Improvements**

  All cmdlet help files have been reviewed and updated for consistency and clarity.
  Significant enhancements to `Get-ZipEntryContent` and `Expand-ZipEntry` help:
    - Added dedicated examples demonstrating password-protected entry handling.
    - Updated parameter descriptions and notes for the new `-Password` parameter.
    - Improved phrasing, removed outdated example output, and ensured uniform formatting across the module.

## 07/02/2025

- Added `AssemblyLoadContext` support for PowerShell 7 (.NET 8.0 or later) to resolve DLL hell by isolating module dependencies. PowerShell 5.1 (.NET Framework) users can't get around this issue due to lack of `AssemblyLoadContext` in that runtime.

## 06/23/2025

- Added commands supporting several algorithms to compress and decompress strings:
    - `ConvertFrom-BrotliString` & `ConvertTo-BrotliString` (using to BrotliSharpLib)
    - `ConvertFrom-DeflateString` & `ConvertTo-DeflateString` (from CLR)
    - `ConvertFrom-ZlibString` & `ConvertTo-ZlibString` (custom implementation)
- Added commands for `.tar` entry management with a reduced set of operations compared to `zip` entry management:
    - `Get-TarEntry`: Lists entries, serving as the main entry point for `TarEntry` cmdlets.
    - `Get-TarEntryContent`: Retrieves the content of a tar entry.
    - `Expand-TarEntry`: Extracts a tar entry to a file.
- Added commands to compress files and folders into `.tar` archives and extract `.tar` archives with various compression algorithms:
    - `Compress-TarArchive` & `Expand-TarArchive`: Supported compression algorithms include `gz`, `bz2`, `zst`, `lz`, and `none` (no compression).
- Removed commands:
    - `Compress-GzipArchive` & `Expand-GzipArchive`: These were deprecated as they only supported single-file compression, which is now better handled by the module’s `.tar` archive functionality. For a workaround to compress or decompress single files using gzip, see [Example 2 in `ConvertTo-GzipString`][example2converttogzipstring], which demonstrates using:

    ```powershell
    [System.Convert]::ToBase64String([System.IO.File]::ReadAllBytes($path)) | ConvertFrom-GzipString
    ```

This update was made possible by the following projects. If you find them helpful, please consider starring their repositories:

- [SharpZipLib](https://github.com/icsharpcode/SharpZipLib)
- [SharpCompress](https://github.com/adamhathcock/sharpcompress)
- [BrotliSharpLib](https://github.com/master131/BrotliSharpLib)
- [ZstdSharp](https://github.com/oleg-st/ZstdSharp)

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

[example2converttogzipstring]: https://github.com/santisq/PSCompression/blob/main/docs/en-US/ConvertTo-GzipString.md#example-2-create-a-gzip-compressed-file-from-a-string
