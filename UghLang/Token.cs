namespace UghLang;

public enum TokenType
{
    None, // other (refrence or undefined)
    StringValue, // string 
    IntValue, // int
    Separator, // ends somthing
    Keyword, // keyword
    Operator, // op
    Comma, // op
    OpenExpression, // opens expression
    CloseExpression, // closes expression
    OpenBlock, // opens block of code
    CloseBlock, // closes block of code
}


public class Token
{
    public static readonly Token NULL_STR = new(string.Empty, TokenType.StringValue);
    public static readonly Token NULL_INT = new("0", TokenType.IntValue) { IntValue = 0 };

    public TokenType Type { get; set; }
    public Keyword? Keyword { get; set; }
    public Operator? Operator { get; set; }

    public string StringValue { get; set; }
    public int IntValue { get; set; }

    public dynamic Value
    {
        get
        {
            return GetDataType() switch
            {
                DataType.String => StringValue,
                DataType.Int => IntValue,
                _ => StringValue,
            };
        }
        set
        {
            switch (GetDataType())
            {
                case DataType.String: StringValue = value.ToString() ?? string.Empty; break;
                case DataType.Int: IntValue = int.Parse(value.ToString() ?? "0"); break;
                default: break;
            }
        }
    }



    public Token(string val, TokenType type) 
    {
        StringValue = val;
        Type = type;

        if (StringValue.TryGetKeyword(out Keyword? t))
        {
            Keyword = t;
            Type = TokenType.Keyword;
        }
        else if(Type == TokenType.Operator)
            Operator = Operation.GetOperator(StringValue);
        else if (Type == TokenType.IntValue && int.TryParse(StringValue, out int result))
            IntValue = result;
    }

    public DataType GetDataType()
    {
        return Type switch 
        { 
            TokenType.StringValue => DataType.String,
            TokenType.IntValue => DataType.Int,
            _ => DataType.Undefined,
        };
    }


    public override string ToString() => $"{nameof(Token)} {{ Value = {StringValue}, Type = {Type}, Keyword = {Keyword}, Operator = {Operator} }}";
}
