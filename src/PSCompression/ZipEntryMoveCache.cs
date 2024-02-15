using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using PSCompression.Extensions;

namespace PSCompression;

internal sealed class ZipEntryMoveCache
{
    private readonly Dictionary<string, Dictionary<string, (ZipEntryBase, string)>> _cache;

    private readonly Dictionary<string, Dictionary<string, string>> _mappings;

    internal ZipEntryMoveCache()
    {
        _cache = new(StringComparer.InvariantCultureIgnoreCase);
        _mappings = new();
    }

    private Dictionary<string, (ZipEntryBase, string)> WithSource(ZipEntryBase entry)
    {
        if (!_cache.ContainsKey(entry.Source))
        {
            _cache[entry.Source] = new();
        }

        return _cache[entry.Source];
    }

    internal void AddEntry(ZipEntryBase entry, string newname) =>
        WithSource(entry).Add(entry.RelativePath, (entry, newname));

    internal Dictionary<string, Dictionary<string, string>> GetMappings(
        ZipArchiveCache cache)
    {
        foreach (var source in _cache)
        {
            _mappings[source.Key] = GetChildMappings(cache, source.Value);
        }

        return _mappings;
    }

    private Dictionary<string, string> GetChildMappings(
        ZipArchiveCache cache,
        Dictionary<string, (ZipEntryBase, string)> pathChanges)
    {
        string newpath;
        Dictionary<string, string> result = new();

        foreach (var pair in pathChanges.OrderByDescending(e => e.Key))
        {
            (ZipEntryBase entry, string newname) = pair.Value;
            if (entry.Type is ZipEntryType.Archive)
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
