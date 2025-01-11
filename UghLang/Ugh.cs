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
    public Function? Function { get; set; }

    public void RegisterName(Name name)
    {
        try { names.Add(name.Key, name); }
        catch (Exception ex) { Debug.Error(ex.Message); }
    }

    public bool TryGetName(string name, out Name value)
    {    
        names.TryGetValue(name, out var n);

        value = n ?? Name.Null; 
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
    public const Name Null = default!;
    public string Key { get; } = name;

    public object Value { get; set; } = val;
    public object AnyValue => Value;


    public T? GetAsOrNull<T>() where T : Name => this as T ?? null;
    public T GetAs<T>() where T : Name => GetAsOrNull<T>() ?? throw new UghException($"{GetType()} is not {typeof(T)}");

    public void Dispose() => GC.SuppressFinalize(this);
    public override string ToString() => $"{nameof(Name)}{{{nameof(Key)} = {Key}; {nameof(Value)} = {Value}}}";
}

public class Variable(string name, object value) : Name(name, value) { }

public abstract class BaseFunction(string name, Ugh ugh) : Name(name, 0)
{
    protected Ugh Ugh => ugh;
    public abstract void Invoke(IEnumerable<IReturnAny> args);
}

public class Function(string name, BlockNode node, ExpressionNode exprs) : BaseFunction(name, node.Ugh)
{
    public BlockNode BlockNode { get; } = node;
    private ExpressionNode ExpressionNode { get; } = exprs;


    public override void Invoke(IEnumerable<IReturnAny> args)
    {
        var lastFun = Ugh.Function;
        Ugh.Function = this;

        List<Variable> localVariables = new(); // TODO: Rework this (optimizations)

        var nodes = ExpressionNode.GetNodes<InitializeNode>();

        var nameNodes = nodes as InitializeNode[] ?? nodes.ToArray();
        var arguments = args as IReturnAny[] ?? args.ToArray();
        
        if (nameNodes.Length != arguments.Length) throw new IncorrectArgumentsException(this);

        for (int i = 0; i < nameNodes.Length; i++) // declaring local variables 
        {
            var nameNode = nameNodes[i];

            Variable v = new(nameNode.Token.StringValue, arguments[i].AnyValue);

            Ugh.RegisterName(v);
            localVariables.Add(v);
        }

        BlockNode.ForceExecute();
        
        localVariables.ForEach(Ugh.FreeName);
        BlockNode.FreeLocalNames();
        
        Ugh.Function = lastFun;
    }
}

public class ModuleFunction(string name, Ugh ugh, MethodInfo method) : BaseFunction(name, ugh)
{
    public override void Invoke(IEnumerable<IReturnAny> args)
    {
        int len = method.GetParameters().Length;
        try
        {
            if (len != args.Count()) throw new IncorrectArgumentsException(this);
            Value = method.Invoke(null, args.Select(item => item.AnyValue).ToArray()) ?? 0;
        }
        catch (Exception ex) { Debug.Error(ex.Message); }
    }
}

