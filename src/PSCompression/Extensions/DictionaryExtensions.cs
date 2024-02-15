using System.Collections.Generic;

namespace PSCompression.Extensions;

internal static class DictionaryExtensions
{
    internal static void Deconstruct<TKey, TValue>(
        this KeyValuePair<TKey, TValue> keyv,
        out TKey key,
        out TValue value)
    {
        key = keyv.Key;
        value = keyv.Value;
    }
}
