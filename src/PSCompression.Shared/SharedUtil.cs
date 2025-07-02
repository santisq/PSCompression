using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.Loader;

namespace PSCompression.Shared;

[ExcludeFromCodeCoverage]
internal sealed class SharedUtil
{
    public static void AddAssemblyInfo(Type type, Dictionary<string, object> data)
    {
        Assembly asm = type.Assembly;

        data["Assembly"] = new Dictionary<string, object?>()
        {
            ["Name"] = asm.GetName().FullName,
            ["ALC"] = AssemblyLoadContext.GetLoadContext(asm)?.Name,
            ["Location"] = asm.Location
        };
    }
}
