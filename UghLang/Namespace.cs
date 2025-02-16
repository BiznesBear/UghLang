using System.Reflection;
using UghLang.Nodes;

namespace UghLang;

public class Namespace : Dictionary<string, Name>
{
    /// <summary>
    /// Checks if name is defined in registry
    /// </summary>
    public bool IsDefined(string name) => ContainsKey(name); 
    
    public void RegisterName(Name name)
    {
        Debug.Print($"Declared + {name.Key}");
        try { Add(name.Key, name); }
        catch (Exception ex) { Debug.Ugh(ex); }
    }

    public bool TryGetName(string name, out Name value) => TryGetValue(name, out value!);

    public Name GetName(string name)
    {
        if (TryGetName(name, out var v)) return v;
        throw new UghException($"Name {name} is not declared");
    }

    public void FreeName(Name name)
    {
        Remove(name.Key);
        name.Dispose();
    }

    public void FreeAll()
    {
        foreach (var v in Values)
            v.Dispose();
        Clear();
    }
    public override string ToString() => $"[{string.Join(", ", Values)}]";
}


/// <summary>
/// Runtime names manager
/// </summary>
public class Rnm : Namespace
{
    /// <summary>
    /// Currently executing function
    /// </summary>
    public Function? Function { get; set; }

    /// <summary>
    /// Contains array of stdlib types 
    /// </summary>
    public Type[] CurrentAssembly { get; } = Assembly.GetExecutingAssembly().GetTypes();


    /// <summary>
    /// Return node will end peek 
    /// </summary>
    public Stack<ASTNode> ReturnStack { get; } = new();

    /// <summary>
    /// Push node to return stack
    /// </summary>
    public void EnterState(ASTNode node) => ReturnStack.Push(node);

    /// <summary>
    /// Pops node from return stack
    /// </summary>
    public void QuitState() => ReturnStack.Pop();
}
