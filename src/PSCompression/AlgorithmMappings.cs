using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        // Brotli
        [".br"] = Algorithm.br,

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

    internal static bool HasExtension(string path) =>
        _mappings.Keys.Any(e => path.EndsWith(e, StringComparison.InvariantCultureIgnoreCase));
}
