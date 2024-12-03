namespace UghLang;
public enum DataType
{
    String,
    Int,
    Undefined
}

public class Variable(string name, Token token) : IDisposable
{
    public static readonly Variable NULL = new("",Token.NULL);
    public string Name { get; } = name;
    public Token Token { get; } = token;

    public void Set(object value) => Token.Value = value;
    public object Get() => Token.Value;

    public void Dispose() => GC.SuppressFinalize(true);
    public override string ToString() => $"{nameof(Variable)} {{ Name = {Name}, Value = {Get()}, Type = {Token.Type} }}";
}