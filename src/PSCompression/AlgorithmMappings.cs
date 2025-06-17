using System;
using System.Collections.Generic;
using System.IO;

namespace PSCompression;

internal static class AlgorithmMappings
{
    private static readonly Dictionary<string, Algorithm> _mappings = new(
        StringComparer.InvariantCultureIgnoreCase)
    {
        [".gz"] = Algorithm.gz,
        [".br"] = Algorithm.br,
        [".bz2"] = Algorithm.bz2,
        [".zst"] = Algorithm.zst,
        [".lz"] = Algorithm.lz,
        [".tar"] = Algorithm.none
    };

    internal static Algorithm Parse(string path) => _mappings[Path.GetExtension(path)];
}
