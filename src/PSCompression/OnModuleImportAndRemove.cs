using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.InteropServices;

/// <summary>
/// OnModuleImportAndRemove is a class that implements the IModuleAssemblyInitializer and IModuleAssemblyCleanup interfaces.
/// This class is used to handle the assembly resolve event when the module is imported and removed.
/// </summary>
[ExcludeFromCodeCoverage]
public class OnModuleImportAndRemove : IModuleAssemblyInitializer, IModuleAssemblyCleanup
{
    /// <summary>
    /// OnImport is called when the module is imported.
    /// </summary>
    public void OnImport()
    {
        if (IsNetFramework())
        {
            AppDomain.CurrentDomain.AssemblyResolve += MyResolveEventHandler;
        }
    }

    /// <summary>
    /// OnRemove is called when the module is removed.
    /// </summary>
    /// <param name="module"></param>
    public void OnRemove(PSModuleInfo module)
    {
        if (IsNetFramework())
        {
            AppDomain.CurrentDomain.AssemblyResolve -= MyResolveEventHandler;
        }
    }

    /// <summary>
    /// MyResolveEventHandler is a method that handles the AssemblyResolve event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    private static Assembly? MyResolveEventHandler(object? sender, ResolveEventArgs args)
    {
        string libDirectory = Path.GetDirectoryName(typeof(OnModuleImportAndRemove).Assembly.Location);
        List<string> directoriesToSearch = [];

        if (!string.IsNullOrEmpty(libDirectory))
        {
            directoriesToSearch.Add(libDirectory);
            if (Directory.Exists(libDirectory))
            {
                IEnumerable<string> dirs = Directory.EnumerateDirectories(
                    libDirectory, "*", SearchOption.AllDirectories);

                directoriesToSearch.AddRange(dirs);
            }
        }

        string requestedAssemblyName = new AssemblyName(args.Name).Name + ".dll";

        foreach (string directory in directoriesToSearch)
        {
            string assemblyPath = Path.Combine(directory, requestedAssemblyName);

            if (File.Exists(assemblyPath))
            {
                try
                {
                    return Assembly.LoadFrom(assemblyPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to load assembly from {assemblyPath}: {ex.Message}");
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Determine if the current runtime is .NET Framework
    /// </summary>
    /// <returns></returns>
    private bool IsNetFramework() => RuntimeInformation.FrameworkDescription
        .StartsWith(".NET Framework", StringComparison.OrdinalIgnoreCase);
}
