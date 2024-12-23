using System.Reflection;
using UghLang.Nodes;

namespace UghLang;

/// <summary>
/// Contains list of reserved runtime names
/// </summary>
public class Ugh
{
    private readonly Dictionary<string, Name> names = new();

    public IReadOnlyDictionary<string, Name> Names => names;

    /// <summary>
    /// Gets execution of current function if exist
    /// </summary>
    public BaseFunction? Function { get; set; }

    public void RegisterName(Name name) => names.Add(name.Key, name);

    public bool TryGetName(string name, out Name value)
    {    
        names.TryGetValue(name, out var n);

        value = n ?? Name.NULL; // assign NULL var if search failed
        return n != null;
    }

    public Name GetName(string name)
    {
        if (TryGetName(name, out var v)) return v;
        else throw new UghException($"Name {name} is not declared");
    }

    public void FreeName(Name name)
    {
        names.Remove(name.Key);
        name.Dispose();
    }

    public void FreeAll()
    {
        foreach (var v in names.Values)
            v.Dispose();
        names.Clear();
    }

}


public abstract class Name(string name, object val) : IDisposable, IReturnAny
{
    public const Name NULL = default;
    public string Key { get; } = name;

    public object Value { get; set; } = val;
    public object AnyValue => Value;


    public T? GetOrNull<T>() where T : Name => this as T ?? null;
    public T Get<T>() where T : Name => GetOrNull<T>() ?? throw new UghException($"Cannot convert {GetType()} to {typeof(T)}");

    public bool TryGet<T>(out T node) where T : Name
    {
        var n = GetOrNull<T>();

        if (n is null)
        {
            node = (T)NULL;
            return false;
        }

        node = n;
        return true;
    }

    public void Dispose() => GC.SuppressFinalize(this);
    public override string ToString() => $"{nameof(Name)}{{{nameof(Key)} = {Key}; {nameof(Value)} = {Value}}}";
}

public class Variable(string name, object value) : Name(name, value) { }

public abstract class BaseFunction(string name, Ugh ugh) : Name(name, 0)
{
    public Ugh Ugh => ugh;
    public void Invoke(IEnumerable<IReturnAny> args)
    {
        Ugh.Function = this;
        OnInvoke(args);
        Ugh.Function = null;
    }

    protected abstract void OnInvoke(IEnumerable<IReturnAny> args);
}

public class Function(string name, TagNode node, ExpressionNode exprs) : BaseFunction(name, node.Ugh)
{
    public TagNode TagNode { get; init; } = node;
    public ExpressionNode ExpressionNode { get; init; } = exprs;


    protected override void OnInvoke(IEnumerable<IReturnAny> args)
    {
        List<Variable> localVariables = new();

        var nodes = ExpressionNode.GetNodes<NameNode>();

        int nodesCount = nodes.Count();

        if (nodesCount != args.Count()) throw new IncorrectArgumentsException(this);

        for (int i = 0; i < nodesCount; i++)
        {
            var nameNode = nodes.ElementAt(i);

            Variable v = new(nameNode.Token.StringValue, args.ElementAt(i).AnyValue);

            Ugh.RegisterName(v);
            localVariables.Add(v);
        }

        TagNode.ForceExecute();

        localVariables.ForEach(Ugh.FreeName);
    }
}

public class ModuleFunction(string name, Ugh ugh, MethodInfo method) : BaseFunction(name, ugh)
{
    protected override void OnInvoke(IEnumerable<IReturnAny> args)
    {
        int len = method.GetParameters().Length;

        if (len != args.Count()) throw new IncorrectArgumentsException(this);

        Value = method.Invoke(null, args.Select(item => item.AnyValue).ToArray()) ?? 0;
    }
}

