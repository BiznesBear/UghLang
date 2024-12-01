namespace UghLang;

public class Master
{
    private readonly List<Variable> variables = new();
    public void DeclareVariable(string name, object value, DataType type)
    {
        Variable var = new(name,value, type);
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
    public Variable GetVariable(string name) => variables.Find(x => x.Name == name) ?? throw new NullReferenceException($"Cannot find variable named {name}");
}

public enum DataType
{
    String,
    Undefined
}
public record Variable(string Name, object Value, DataType Type) : IDisposable
{
    public void Dispose() { }
}
