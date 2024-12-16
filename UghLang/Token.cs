namespace UghLang;

public enum TokenType
{
    Name, // can be everything 
    Keyword, // keyword

    Operator, // op
    Separator, // ends somthing

    OpenExpression, // opens expression
    CloseExpression, // closes expression
    OpenBlock, // opens block of code
    CloseBlock, // closes block of code

    StringValue, // string 
    IntValue, // int
    BoolValue, // boolean
    FloatValue, // float
}


public class Token 
{
    public TokenType Type { get; set; }

    public string StringValue { get; set; }
    public int IntValue => int.Parse(StringValue);
    public float FloatValue => float.Parse(StringValue, System.Globalization.CultureInfo.InvariantCulture);
    public bool BoolValue => bool.Parse(StringValue);


    public Keyword? Keyword { get; set; }
    public Operator Operator => Operation.GetOperator(StringValue);


    /// <summary>
    /// Return value.
    /// </summary>
    public object Value // TODO: Remove this
    {
        get
        {
            return Type switch
            {
                TokenType.StringValue => StringValue,
                TokenType.IntValue => IntValue,
                TokenType.BoolValue => BoolValue,
                TokenType.FloatValue => FloatValue,
                _ => StringValue,
            };
        }

    }

    /// <summary>
    /// Assign, and check values.
    /// </summary>
    /// <param name="val">Input for token</param>
    /// <param name="type">Interpretation of value</param>
    public Token(string val, TokenType type) 
    {
        StringValue = val;
        Type = type;

        if (StringValue.TryGetKeyword(out Keyword kw, out TokenType t) && Type == TokenType.Name) // check if none type can be keyword
        {
            Keyword = kw;
            Type = t;
        }
    }


    public override string ToString() => $"{nameof(Token)} {{ Value = {StringValue} | Type = {Type} | Keyword = {Keyword} }}";
}
