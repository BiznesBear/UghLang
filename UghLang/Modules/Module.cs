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
    public static ModuleInfo LoadModule(string moduleName, Type[]? assembly)
    {
        Dictionary<string, MethodInfo> methods = new();
        Dictionary<string, FieldInfo> fields = new();

        var types = assembly is null ? Assembly.GetExecutingAssembly().GetTypes() : assembly;

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

        Debug.Print($"Loaded {methods.Count} methods and {fields.Count} fields");

        return new(methods, fields);
    }

    public static Type[] LoadAssembly(string path) => Assembly.LoadFrom(path).GetTypes();
}