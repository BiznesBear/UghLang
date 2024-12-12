namespace UghLang;

public enum TokenType
{
    None, // other (refrence or undefined)
    StringValue, // string 
    IntValue, // int
    BoolValue, // int
    Separator, // ends somthing
    Keyword, // keyword
    Operator, // op
    OpenExpression, // opens expression
    CloseExpression, // closes expression
    OpenBlock, // opens block of code
    CloseBlock, // closes block of code
}


public class Token 
{
    public TokenType Type { get; set; }

    public string StringValue { get; set; }
    public int IntValue => int.Parse(StringValue);
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
            return GetDataType() switch
            {
                DataType.String => StringValue,
                DataType.Int => IntValue,
                DataType.Bool => BoolValue,
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

        if (StringValue.TryGetKeyword(out Keyword kw, out TokenType t))
        {
            Keyword = kw;
            Type = t;
        }
    }



    public DataType GetDataType()
    {
        return Type switch 
        { 
            TokenType.StringValue => DataType.String,
            TokenType.IntValue => DataType.Int,
            TokenType.BoolValue => DataType.Bool,
            _ => DataType.Undefined,
        };
    }

    public override string ToString() => $"{nameof(Token)} {{ Value = {StringValue}, Type = {Type}, Keyword = {Keyword} }}";
}
