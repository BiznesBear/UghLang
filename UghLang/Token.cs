namespace UghLang;

public enum TokenType
{
    None, // other
    StringValue, // string 
    IntValue, // int
    Separator, // ends current block of code
    Keyword, // keyword
    Operator,
}


public class Token
{
    public string StringValue { get; set; }
    public int IntValue { get; set; }

    public object Value
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

    public TokenType Type { get; set; }
    public Keyword? Keyword { get; set; }
    public Operator? Operator { get; set; }

    public static readonly Token NULL = new(string.Empty, TokenType.None);

    public Token(string val, TokenType type) 
    {
        StringValue = val;
        Type = type;
        if (StringValue.TryGetKeyword(out Keyword? t))
        {
            Keyword = t;
            Type = TokenType.Keyword;
        }
        if(Type == TokenType.Operator)
            Operator = Operation.GetOperator(StringValue);
        if (Type == TokenType.IntValue && int.TryParse(StringValue, out int result))
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
