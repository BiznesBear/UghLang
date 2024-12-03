namespace UghLang;

/// <summary>
/// Variable managment
/// </summary>
public class Master
{
    
    private readonly List<Variable> variables = new();

    public void DeclareVariable(string name, Token token)
    {
        Variable var = new(name,token);
        variables.Add(var);

        // TODO: Testing
        Console.WriteLine(var);
    }

    public void FreeVariable(string name)
    {
        var v = GetVariable(name);
        variables.Remove(v);
        v.Dispose();
    }
    public void FreeAllVariables()
    {
        variables.ForEach(d => d.Dispose());
        variables.Clear();
    }

    public bool TryGetVariable(string name, out Variable var)
    {
        var v = variables.Find(x => x.Name == name);
        var = v ?? Variable.NULL;
        return v != null;
    }

    public Variable GetVariable(string name)
    {
        if (TryGetVariable(name, out var v)) return v;
        else throw new NullReferenceException($"Cannot find variable named '{name}'");
    }
    public bool Contains(string name) => TryGetVariable(name, out var v);
}
