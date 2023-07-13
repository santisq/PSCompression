using System;
using System.Management.Automation;
using System.Reflection;

namespace PSCompression;

internal static class PSVersionHelper
{
    private static bool? _isCore;

    internal static bool IsCoreCLR => _isCore ??= IsCore();

    private static bool IsCore()
    {
        PropertyInfo property = typeof(PowerShell)
            .Assembly.GetType("System.Management.Automation.PSVersionInfo")
            .GetProperty(
                "PSVersion",
                BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

        return (Version)property.GetValue(property) is not { Major: 5, Minor: 1 };
    }
}
