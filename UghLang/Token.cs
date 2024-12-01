namespace UghLang;

public enum TokenType
{
    None, // other
    StringValue, // string 
    Separator, // ends current block of code
    Keyword, 
}


public record Token(string Value, TokenType Type, Keyword? Keyword)
{
    public static readonly Token NULL = new(string.Empty, TokenType.None);
    public Token(string val, TokenType type) : this(val, type, null)
    {
        if (Value.GetKeyword(out Keyword? t))
        {
            Keyword = t;
            Type = TokenType.Keyword;
        }
    }

    public DataType GetDataType() => Type == TokenType.StringValue ? DataType.String : DataType.Undefined;
    public Refrence GetValue() => new(Value, Type == TokenType.StringValue ? RefrenceType.Value : RefrenceType.Other);
}
