using UghLang.Nodes;
namespace UghLang;


/// <summary>
/// Runtime names manager
/// </summary>
public class Ugh
{
    public IReadOnlyDictionary<string, Name> Names => names;

    /// <summary>
    /// Currently executing function
    /// </summary>
    public Function? Function { get; set; }
    public Type[]? StdAssembly { get; set; }  

    private readonly Dictionary<string, Name> names = new();
    


    public BlockNode? ReturnBlock { get; private set; } // return will end this node 

    private BlockNode? lastReturnBlock;

    public void SetReturnBlock(BlockNode? node)
    {
        if(node is null) 
            ReturnBlock = lastReturnBlock;
        else { ReturnBlock = node; lastReturnBlock = node; }
    }

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
