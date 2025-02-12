using System.Reflection;
using UghLang.Nodes;
namespace UghLang;


/// <summary>
/// Runtime names manager
/// </summary>
public class Ugh
{
    /// <summary>
    /// Dictionary with registred names
    /// </summary>
    public IReadOnlyDictionary<string, Name> Names => names;


    /// <summary>
    /// Currently executing function
    /// </summary>
    public Function? Function { get; set; }

    /// <summary>
    /// Contains array of stdlib types 
    /// </summary>
    public Type[] CurrentAssembly { get; }


    /// <summary>
    /// Return node will end peek 
    /// </summary>
    public Stack<ASTNode> ReturnStack { get; } = new();


    private readonly Dictionary<string, Name> names = new();
   
    public Ugh() 
    {
        CurrentAssembly = Assembly.GetExecutingAssembly().GetTypes();
    }

    /// <summary>
    /// Push node to return stack
    /// </summary>
    public void EnterState(ASTNode node) => ReturnStack.Push(node);

    /// <summary>
    /// Pops node from return stack
    /// </summary>
    public void QuitState() => ReturnStack.Pop();

    /// <summary>
    /// Checks if name is defined in registry
    /// </summary>
    public bool IsDefined(string name) => names.ContainsKey(name); 
    
    public void RegisterName(Name name)
    {
        try { names.Add(name.Key, name); }
        catch (Exception ex) { Debug.Ugh(ex); }
    }

    public bool TryGetName(string name, out Name value) => names.TryGetValue(name, out value!);

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
