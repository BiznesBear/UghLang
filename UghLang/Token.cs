namespace UghLang;

public enum TokenType : byte
{
    Name, // undefined 
    Keyword, 

    Operator, 
    Separator, // ; - goes back to master branch 
    
    // braces
    OpenExpression, 
    CloseExpression, 

    OpenBlock, 
    CloseBlock, 

    OpenIndex, 
    CloseIndex, 

    // values
    StringValue, 
    IntValue, 
    BoolValue, 
    FloatValue, 

    // other
    Comma,
    Colon,
    Preload,
    Not,
    Pi,
    Lambda,
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
   
    public Operator Operator
    {
        get
        {
            return StringValue switch
            {
                "=" => Operator.Equals,
                "+" or "+=" => Operator.Plus,
                "-" or "-=" => Operator.Minus,
                "*" or "*=" => Operator.Multiply,
                "/" or "/=" => Operator.Divide,
                "%" or "%=" => Operator.DivideRest,

                "**" => Operator.Power,
                "//" => Operator.Sqrt,

                // BOOLEAN
                "==" => Operator.DoubleEquals,
                "!=" => Operator.NotEquals,
                "<" => Operator.Less,
                ">" => Operator.Higher,
                "<=" => Operator.LessEquals,
                ">=" => Operator.HigherEquals,
                "&&" => Operator.And,
                "||" => Operator.Or,

                _ => throw new UghException("Cannot find operator " + StringValue)
            };
        }
        
    }


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
