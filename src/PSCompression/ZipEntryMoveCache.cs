using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using PSCompression.Abstractions;
using PSCompression.Extensions;

namespace PSCompression;

internal sealed class ZipEntryMoveCache
{
    private readonly Dictionary<string, Dictionary<string, EntryWithPath>> _cache;

    private readonly Dictionary<string, Dictionary<string, string>> _mappings;

    internal ZipEntryMoveCache()
    {
        _cache = new(StringComparer.InvariantCultureIgnoreCase);
        _mappings = [];
    }

    private Dictionary<string, EntryWithPath> WithSource(ZipEntryBase entry)
    {
        if (!_cache.ContainsKey(entry.Source))
        {
            _cache[entry.Source] = [];
        }

        return _cache[entry.Source];
    }

    internal bool IsDirectoryEntry(string source, string path) =>
        _cache[source].TryGetValue(path, out EntryWithPath entryWithPath)
            && entryWithPath.ZipEntry.Type is EntryType.Directory;

    internal void AddEntry(ZipEntryBase entry, string newname) =>
        WithSource(entry).Add(entry.RelativePath, new(entry, newname));

    internal IEnumerable<(string, PathWithType)> GetPassThruMappings()
    {
        foreach (var source in _cache)
        {
            foreach ((string path, EntryWithPath entryWithPath) in source.Value)
            {
                yield return (
                    source.Key,
                    new PathWithType(
                        _mappings[source.Key][path],
                        entryWithPath.ZipEntry.Type));
            }
        }
    }

    internal Dictionary<string, Dictionary<string, string>> GetMappings(
        ZipArchiveCache<ZipArchive> cache)
    {
        foreach (var source in _cache)
        {
            _mappings[source.Key] = GetChildMappings(cache, source.Value);
        }

        return _mappings;
    }

    private Dictionary<string, string> GetChildMappings(
        ZipArchiveCache<ZipArchive> cache,
        Dictionary<string, EntryWithPath> pathChanges)
    {
        string newpath;
        Dictionary<string, string> result = [];

        foreach (var pair in pathChanges.OrderByDescending(e => e.Key))
        {
            (ZipEntryBase entry, string newname) = pair.Value;
            if (entry.Type is EntryType.Archive)
            {
                newpath = ((ZipEntryFile)entry).ChangeName(newname);
                result[pair.Key] = newpath;
                continue;
            }

            ZipEntryDirectory dir = (ZipEntryDirectory)entry;
            newpath = dir.ChangeName(newname);
            result[pair.Key] = newpath;
            Regex re = new(
                Regex.Escape(dir.RelativePath),
                RegexOptions.Compiled | RegexOptions.IgnoreCase);

            foreach (ZipArchiveEntry key in dir.GetChilds(cache[dir.Source]))
            {
                string child = result.ContainsKey(key.FullName)
                    ? result[key.FullName]
                    : key.FullName;

                result[key.FullName] = re.Replace(child, newpath);
            }
        }

        return result;
    }
}
