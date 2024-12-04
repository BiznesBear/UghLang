namespace UghLang;


public class Master
{
    private readonly List<Variable> variables = new();

    public void DeclareVariable(Variable var)
    {
        variables.Add(var);

        // TODO: Testing
        Console.WriteLine(var);
    }

    public void FreeVariable(Token token)
    {
        var v = GetVariable(token);
        variables.Remove(v);
        v.Dispose();
    }
    public void FreeAllVariables()
    {
        variables.ForEach(d => d.Dispose());
        variables.Clear();
    }

    public bool TryGetVariable(Token token, out Variable var)
    {
        var = Variable.NULL;
        if (token.Type != TokenType.None) return false;

        var v = variables.Find(x => x.Name == token.StringValue);
       
        var = v ?? Variable.NULL;
        return v != null;
    }

    public Variable GetVariable(Token token)
    {
        if (TryGetVariable(token, out var v)) return v;
        else throw new NullReferenceException($"Cannot find variable named '{token.StringValue}'");
    }
    public bool Contains(Token token) => TryGetVariable(token, out var v);
}
