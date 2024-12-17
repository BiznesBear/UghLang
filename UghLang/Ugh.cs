using UghLang.Nodes;

namespace UghLang;


public class Ugh
{
    private readonly Dictionary<string, Name> names = new();

    public IReadOnlyDictionary<string, Name> Names => names;

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


    public T Get<T>() where T : Name => this as T ?? throw new UghException($"Cannot convert {GetType()} to {typeof(T)}");

    public void Dispose() => GC.SuppressFinalize(this);
    public override string ToString() => $"{nameof(Name)}{{{nameof(Key)} = {Key}; {nameof(Value)} = {Value}}}";
}

public class Variable(string name, object value) : Name(name, value) { }
public class Function(string name, TagNode node, ExpressionNode exprs) : Name(name, 0)
{
    private TagNode TagNode { get; set; } = node;
    private ExpressionNode ExpressionNode { get; set; } = exprs;

    private Ugh Ugh => TagNode.Ugh;

    public void Invoke(IEnumerable<IReturnAny> args) 
    {
        List<Variable> localVariables = new();

        var nodes = ExpressionNode.GetNodes<NameNode>();

        int nodesCount = nodes.Count();
        if (nodesCount != args.Count()) throw new IncorrectAmountOfArgumentException(this);

        for (int i = 0; i < nodesCount; i++)
        {
            var nameNode = nodes.ElementAt(i);
            Variable v = new(nameNode.Token.StringValue, args.ElementAt(i).AnyValue);
            
            Ugh.RegisterName(v);
            localVariables.Add(v);
        }

        TagNode.Executable = true;
        TagNode.Execute();
        TagNode.Executable = false;

        localVariables.ForEach(Ugh.FreeName);
    }
}
