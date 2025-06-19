using System;
using System.Collections.Generic;
using System.IO;

namespace PSCompression;

internal static class AlgorithmMappings
{
    private static readonly Dictionary<string, Algorithm> _mappings = new(
        StringComparer.InvariantCultureIgnoreCase)
    {
        // Gzip
        [".gz"] = Algorithm.gz,
        [".gzip"] = Algorithm.gz,
        [".tgz"] = Algorithm.gz,

        // Bzip2
        [".bz2"] = Algorithm.bz2,
        [".bzip2"] = Algorithm.bz2,
        [".tbz2"] = Algorithm.bz2,
        [".tbz"] = Algorithm.bz2,

        // Zstandard
        [".zst"] = Algorithm.zst,

        // Lzip
        [".lz"] = Algorithm.lz,

        // No compression
        [".tar"] = Algorithm.none
    };

    internal static Algorithm Parse(string path) =>
        _mappings.TryGetValue(Path.GetExtension(path), out Algorithm value)
            ? value : Algorithm.none;
}
