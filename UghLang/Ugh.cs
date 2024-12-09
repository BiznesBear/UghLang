namespace UghLang;


public class Ugh
{
    private readonly List<Variable> variables = new();
    public void DeclareVariable(Variable var)
    {
        variables.Add(var);
        Debug.Print(var); // For testing only
    }

    public void FreeVariable(Variable variable)
    {
        variables.Remove(variable);
        variable.Dispose();
    }
    public void FreeVariable(Token token) => FreeVariable(GetVariable(token));
    public void FreeAllVariables()
    {
        variables.ForEach(v => v.Dispose());
        variables.Clear();
    }

    public bool TryGetVariable(Token token, out Variable var)
    {
        var = Variable.NULL; // defalut return
        if (token.Type != TokenType.None) return false; // check if token can even be variable

        var v = variables.Find(x => x.Name == token.StringValue); // search for variable

        var = v ?? Variable.NULL; // assign NULL var if search failed
        return v != null;
    }

    public Variable GetVariable(Token token)
    {
        if (TryGetVariable(token, out var v)) return v;
        else throw new NullVariableRefrenceException(token);
    }
    public bool Contains(Token token) => TryGetVariable(token, out var v);
    public bool Contains(Variable variable) => variables.Contains(variable);
}
