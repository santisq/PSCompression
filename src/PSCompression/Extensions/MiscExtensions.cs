using System.Collections.Generic;
using System.Net;
using System.Security;

namespace PSCompression.Extensions;

internal static class MiscExtensions
{
    internal static void Deconstruct<TKey, TValue>(
        this KeyValuePair<TKey, TValue> keyv,
        out TKey key,
        out TValue value)
    {
        key = keyv.Key;
        value = keyv.Value;
    }

    internal static string AsPlainText(this SecureString secureString) =>
        new NetworkCredential(string.Empty, secureString).Password;
}
