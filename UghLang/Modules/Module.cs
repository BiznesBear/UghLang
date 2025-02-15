using System.Reflection;

namespace UghLang.Modules;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class Module(string name) : Attribute
{
    public string Name => name;
}

public record ModuleInfo(Dictionary<string, MethodInfo> Methods, Dictionary<string, FieldInfo> Fields);
public static class ModuleLoader
{
    public static ModuleInfo LoadModule(string moduleName, Type[] assembly)
    {
        foreach (var type in assembly)
            if (type.Name == moduleName || type.GetCustomAttribute<Module>()?.Name == moduleName)
            {
                var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static).ToDictionary(m => m.Name);
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy).Where(f => f.IsLiteral).ToDictionary(f => f.Name);

                Debug.Info($"Loaded {methods.Count} methods and {fields.Count} constants from {moduleName}");

                return new ModuleInfo(methods, fields);
            }
        throw new UghException("Cannot find module " +  moduleName);        
    }

    public static Type[] LoadAssembly(string path) => Assembly.LoadFrom(path).GetTypes();
}