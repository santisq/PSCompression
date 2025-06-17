using PSCompression.Abstractions;

namespace PSCompression;

internal record struct EntryWithPath(ZipEntryBase ZipEntry, string Path);

internal record struct PathWithType(string Path, EntryType EntryType);
