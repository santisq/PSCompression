using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace PSCompression.Shared;

public sealed class LoadContext : AssemblyLoadContext
{
    private static LoadContext? _instance;

    private readonly static object s_sync = new();

    private readonly Assembly _thisAssembly;

    private readonly AssemblyName _thisAssemblyName;

    private readonly Assembly _moduleAssembly;

    private readonly string _assemblyDir;

    private LoadContext(string mainModulePathAssemblyPath)
        : base(name: "PSCompression", isCollectible: false)
    {
        _assemblyDir = Path.GetDirectoryName(mainModulePathAssemblyPath) ?? "";
        _thisAssembly = typeof(LoadContext).Assembly;
        _thisAssemblyName = _thisAssembly.GetName();
        _moduleAssembly = LoadFromAssemblyPath(mainModulePathAssemblyPath);
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        if (AssemblyName.ReferenceMatchesDefinition(_thisAssemblyName, assemblyName))
        {
            return _thisAssembly;
        }

        string asmPath = Path.Join(_assemblyDir, $"{assemblyName.Name}.dll");
        if (File.Exists(asmPath))
        {
            return LoadFromAssemblyPath(asmPath);
        }

        return null;
    }

    public static Assembly Initialize()
    {
        LoadContext? instance = _instance;
        if (instance is not null)
        {
            return instance._moduleAssembly;
        }

        lock (s_sync)
        {
            if (_instance is not null)
            {
                return _instance._moduleAssembly;
            }

            string assemblyPath = typeof(LoadContext).Assembly.Location;
            string assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
            string moduleName = assemblyName[..^7];
            string modulePath = Path.Combine(
                Path.GetDirectoryName(assemblyPath)!,
                $"{moduleName}.dll");

            _instance = new LoadContext(modulePath);
            return _instance._moduleAssembly;
        }
    }
}
