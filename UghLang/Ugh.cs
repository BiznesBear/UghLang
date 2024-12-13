namespace UghLang;


public class Ugh
{
    private readonly List<Variable> variables = new();
    private readonly List<Function> functions = new();

    #region Variable
    public void InitializeVariable(Variable var)
    {
        variables.Add(var);
        Debug.Print("Declared " + var); // For testing only
    }

    public bool TryGetVariable(Token token, out Variable var)
    {
        var = Variable.NULL;
        if (token.Type != TokenType.None) return false; // check if token can even be variable

        var v = variables.Find(x => x.Name == token.StringValue); // search for variable

        var = v ?? Variable.NULL; // assign NULL var if search failed
        return v != null;
    }

    public Variable GetVariable(Token token)
    {
        if (TryGetVariable(token, out var v)) return v;
        else throw new NullReferenceException("Cannot find function named " + token.StringValue);
    }
    #endregion

    #region Function
    public void DeclareFunction(Function fun)
    {
        functions.Add(fun);
        Debug.Print("Initialized " + fun); // For testing only
    }

    public bool TryGetFunction(string name, out Function func)
    {       
        var f = functions.Find(x => x.Name == name); // search for variable

        func = f ?? Function.NULL; // assign NULL var if search failed
        return f != null;
    }

    public Function GetFunction(string name)
    {
        if (TryGetFunction(name, out var v)) return v;
        else throw new NullReferenceException("Cannot find function named " + name);
    }
    #endregion

    #region Free
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
    #endregion 
}
public record Function(string Name, TagNode Node)
{
    public const Function NULL = default;

    public void Invoke() // TODO: Add invoking with arguments
    {
        Node.Execute();
    }
}