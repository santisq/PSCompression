## ZIP EXAMPLES

### Example 1: Compress all `.ext` files from a specific folder

Get-ChildItem .\path -Recurse -Filter *.ext |
    Compress-ZipArchive -DestinationPath dest.zip

### Example 2: Compress all `.txt` files contained in all folders in the Current Directory

Compress-ZipArchive .\*\*.txt -DestinationPath dest.zip

### Example 3: Compress all `.ext` and `.ext2` from a specific folder

Compress-ZipArchive .\*.ext, .\*.ext2 -DestinationPath dest.zip

### Example 4: Compress a folder using `Fastest` Compression Level

Compress-ZipArchive .\path -Destination myPath.zip -CompressionLevel Fastest

### Example 5: Compressing all directories in `.\Path`

Get-ChildItem .\path -Recurse -Directory |
    Compress-ZipArchive -DestinationPath dest.zip

### Example 6: Replacing an existing Zip Archive

Compress-ZipArchive -Path .\path -DestinationPath dest.zip -Force

### Example 7: Adding and updating new entries to an existing Zip Archive

Get-ChildItem .\path -Recurse -Directory |
    Compress-ZipArchive -DestinationPath dest.zip -Update
