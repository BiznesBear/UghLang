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
    public static ModuleInfo LoadModule(string moduleName, string? path = null)
    {
        Dictionary<string, MethodInfo> methods = new();
        Dictionary<string, FieldInfo> fields = new();
        var types = LoadTypes(path);

        foreach (var type in types)
        {
            var moduleAttr = type.GetCustomAttribute<Module>();
            if (moduleAttr != null && moduleAttr.Name == moduleName)
            {
                var classMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
                var classFields = type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                foreach (var method in classMethods)
                    methods[method.Name] = method;
                foreach (var field in classFields)
                    if (field.IsLiteral && !field.IsInitOnly) fields[field.Name] = field;
                break;
            }
        }

        if (methods.Count == 0)
            throw new InvalidOperationException($"Module '{moduleName}' not found.");

        return new(methods, fields);
    }


    public static Type[] LoadTypes(string? path = null) =>
        path is null ? Assembly.GetExecutingAssembly().GetTypes() : Assembly.LoadFrom(path).GetTypes();
}