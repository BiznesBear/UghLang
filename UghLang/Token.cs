namespace UghLang;

public enum TokenType : byte
{
    Name, // can be everything 
    Keyword, // keyword

    Operator, // op
    Separator, // ends somthing

    OpenExpression, // opens expression
    CloseExpression, // closes expression

    OpenBlock, // opens block of code
    CloseBlock, // closes block of code

    OpenList, // opens list
    CloseList, // closes list

    StringValue, // string 
    IntValue, // int
    BoolValue, // boolean
    FloatValue, // float

    Comma,
    Colon,
    Preload,
    Pi,

    EndOfFile
}


public class Token 
{
    public TokenType Type { get; }
    public string StringValue { get; }

    public int IntValue => int.Parse(StringValue);
    public bool BoolValue => bool.Parse(StringValue);
    public float FloatValue => float.Parse(StringValue, System.Globalization.CultureInfo.InvariantCulture);
    public double DoubleValue => double.Parse(StringValue, System.Globalization.CultureInfo.InvariantCulture);
   
    public Operator Operator => BinaryOperation.GetOperator(StringValue);
    public Keyword? Keyword { get; } // TODO: Remove this 

    public Token(string val, TokenType type) 
    {
        StringValue = val;
        Type = type;

        if (Type == TokenType.Name && StringValue.TryGetKeyword(out Keyword kw, out TokenType t)) // check if none type can be keyword
        {
            Keyword = kw;
            Type = t;
        }
    }

    public override string ToString() => $"{nameof(Token)} {{ Value = {StringValue} | Type = {Type} | Keyword = {Keyword} }}";
}
