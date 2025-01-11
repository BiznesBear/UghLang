using System.Reflection;

namespace UghLang.Modules;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
public sealed class Module(string name) : Attribute
{
    public string Name => name;
}

public static class ModuleLoader
{
    public static Dictionary<string, MethodInfo> LoadModuleMethods(string moduleName)
    {
        var methods = new Dictionary<string, MethodInfo>();
        
        var types = Assembly.GetExecutingAssembly().GetTypes();
        
        foreach (var type in types)
        {
            var moduleAttr = type.GetCustomAttribute<Module>();
            if (moduleAttr != null && moduleAttr.Name == moduleName)
            {
                var classMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);

                foreach (var method in classMethods)
                    methods[method.Name] = method;
                break;
            }
        }

        if (methods.Count == 0)
            throw new InvalidOperationException($"Module '{moduleName}' not found.");

        return methods;
    }
}

