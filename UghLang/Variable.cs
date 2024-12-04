namespace UghLang;
public enum DataType
{
    String,
    Int,
    Undefined
}

public class Variable(Token baseToken, Token stdValueToken) : IDisposable
{
    public static readonly Variable NULL = new(Token.NULL_STR, Token.NULL_STR);
    public string Name { get; } = baseToken.StringValue;
    public Token Token { get; } = stdValueToken;

    public void Set(object value) => Token.Value = value;
    public object Get() => Token.Value;

    public void Dispose() => GC.SuppressFinalize(true);
    public override string ToString() => $"{nameof(Variable)} {{ Name = {Name}, Value = {Get()}, Type = {Token.Type} }}";
}