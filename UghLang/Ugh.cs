﻿using System.Reflection;
using UghLang.Nodes;
namespace UghLang;

public class Namespace : Dictionary<string, Name>;

/// <summary>
/// Script runner 
/// </summary>
public class Ugh
{
    private readonly List<string> definitions = [];

    public Type[]? StdAssembly { get; set; }  
    public IReadOnlyDictionary<string, Name> Names => names;

    /// <summary>
    /// Currently executed function
    /// </summary>
    public Function? Function { get; set; }

    /// <summary>
    /// ReturnNode will end this block node
    /// </summary>
    public BlockNode? ReturnBlock { get; private set; }

    private readonly Namespace names = new();
    private BlockNode? lastReturnBlock;

    public void SetReturnBlock(BlockNode? node)
    {
        if(node is null) ReturnBlock = lastReturnBlock;
        else { ReturnBlock = node; lastReturnBlock = node; }
    }

    public void Define(string name) => definitions.Add(name); 
    public bool IsDefined(string name) => definitions.Contains(name); 
    
    public void RegisterName(Name name)
    {
        try { names.Add(name.Key, name); }
        catch (Exception ex) { Debug.Error(ex); }
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

/// <summary>
/// Names values with GetOnly can't be set
/// </summary>
public interface IGetOnly;

public abstract class Name(string name, object val) : Namespace, IDisposable, IReturnAny
{
    public const Name Null = default!;
    public string Key { get; } = name;


    protected object value = val;
    public object Value { get => value; set { if (this is not IGetOnly) this.value = value; } }  
    public object AnyValue => Value;


    public T? GetAsOrNull<T>() where T : Name => this as T ?? null;
    public T GetAs<T>() where T : Name => GetAsOrNull<T>() ?? throw new UghException($"{GetType()} is not {typeof(T)}");

    public void Dispose() => GC.SuppressFinalize(this);
    public override string ToString() => $"{nameof(Name)}{{{nameof(Key)} = {Key}; {nameof(Value)} = {Value}}}";
}

public class Variable(string name, object value) : Name(name, value) { }
public class Constant(string name, object value) : Variable(name, value), IGetOnly, IConstantValue
{
    public object ConstantValue => AnyValue;
}

public class AssemblyConst(string name, Type[] assembly) : Constant(name, assembly) 
{ 
    public Type[] Assembly { get; } = assembly;
}

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

        var nodes = ExpressionNode.GetNodes<NameNode>();

        var nameNodes = nodes as NameNode[] ?? nodes.ToArray();
        var arguments = args as IReturnAny[] ?? args.ToArray();
        
        if (nameNodes.Length != arguments.Length) throw new IncorrectArgumentsException(this);

        for (int i = 0; i < nameNodes.Length; i++) // declaring local variables 
        {
            var nameNode = nameNodes[i];

            Variable v = new(nameNode.Token.StringValue, arguments[i].AnyValue);

            Ugh.RegisterName(v);
            BlockNode.LocalNames.Add(v);
        }

        BlockNode.ForceExecute();
        BlockNode.FreeLocalNames();
        
        Ugh.Function = lastFun;
    }
}

public class ModuleFunction(string name, Ugh ugh, MethodInfo method) : BaseFunction(name, ugh)
{
    public override void Invoke(IEnumerable<IReturnAny> args)
    {
        Value = method.Invoke(null, args.Select(item => item.AnyValue).ToArray()) ?? 0;
    }
}
