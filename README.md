# Compress-File

## Description

PowerShell function that overcomes the limitation that the built-in cmdlet [`Compress-Archive`](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.archive/compress-archive?view=powershell-7.2) has:

> The `Compress-Archive` cmdlet uses the Microsoft .NET API [`System.IO.Compression.ZipArchive`](https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.ziparchive?view=net-6.0) to compress files. The maximum file size is 2 GB because there's a limitation of the underlying API.

The easy workaround would be to use the [`ZipFile.CreateFromDirectory` Method](https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.zipfile.createfromdirectory?view=net-6.0#system-io-compression-zipfile-createfromdirectory(system-string-system-string)). However, there are 3 limitations while using this static method:

   1. The source __must be a directory__, a single file cannot be compressed.
   2. All files (recursively) on the source folder __will be compressed__, we can't pick / filter files to compress.
   3. It's not possible to __Update__ the entries of an existing Zip Archive.

This function should be able to handle the same as `CreateFromDirectory` method but also allow us to filter a folder for specific files to compress and also keep the file / folder structure untouched.

## Parameters

| Name | Description |
| ---  | --- |
| `-Path` | Absolute or relative path for the File or Folder to be compressed |
| `-DestinationPath` | The destination path to the Zip file
| `-CompressionLevel` &nbsp; &nbsp; | Define the compression level that should be used. See [CompressionLevel Enum](https://docs.microsoft.com/en-us/dotnet/api/system.io.compression.compressionlevel?view=net-6.0) for details
| `-Update` | Updates Zip entries and adds new entries to an existing Zip file
| `-Force` | Replaces an existing Zip file with a new one. All Zip contents will be lost
| `-PassThru` | Outputs the object representing the compressed file. The function produces no output by default

## Performance Measurements

Below is a performance comparison between [`Compress-Archive`](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.archive/compress-archive?view=powershell-7.2) and this function. Source code for the performance tests in [performance_tests.ps1](performance_tests.ps1).

Below are the test results in [PowerShell Core](https://github.com/PowerShell/PowerShell) where the built-in cmdlet is [known for having performance issues](https://github.com/PowerShell/Microsoft.PowerShell.Archive/issues/78).

### Average Reults

```none
Test                        Average  RelativeSpeed
----                        -------  -------------
Compress-File (Optimal)     1178.75  1x
Compress-Archive (Optimal)  34179.89 29.00x
```

### Results per Test Run

```none
TestRun Test                        TotalMilliseconds
------- ----                        -----------------
      3 Compress-File (Optimal)              1132.38
      4 Compress-File (Optimal)              1151.72
      2 Compress-File (Optimal)              1156.69
      5 Compress-File (Optimal)              1157.54
      1 Compress-File (Optimal)              1295.44
      2 Compress-Archive (Optimal)           33884.40
      4 Compress-Archive (Optimal)           33907.80
      3 Compress-Archive (Optimal)           33940.75
      5 Compress-Archive (Optimal)           34264.44
      1 Compress-Archive (Optimal)           34902.04
```

## Examples

- Compress all `.ext` files from a specific folder:

```powershell
Get-ChildItem .\path -Recurse -Filter *.ext |
    Compress-File -DestinationPath dest.zip
```

- Compress all `.ext` and `.ext2` from a specific folder:

```powershell
Get-ChildItem .\path -Recurse -Include *.ext, *.ext2 |
    Compress-File -DestinationPath dest.zip
```

- Compress a folder using _Fastest_ Compression Level:

```powershell
Compress-File .\path -Destination myPath.zip -CompressionLevel Fastest
```

- Compressing all directories in `.\Path`:

```powershell
Get-ChildItem .\path -Recurse -Directory |
    Compress-File -DestinationPath dest.zip
```

- Replacing an existing Zip Archive:

```powershell
Compress-File -Path .\path -DestinationPath dest.zip -Force
```

- Adding and updating new entries to an existing Zip Archive:

```powershell
Get-ChildItem .\path -Recurse -Directory |
    Compress-File -DestinationPath dest.zip -Update
```
