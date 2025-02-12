namespace UghLang;

public enum TokenType : byte
{
    Name, // undefined 

    Operator, 
    Separator, // ; goes back to master branch 
    
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
    DoubleValue,
    ByteValue,
    NullValue,

    // other
    Comma,
    Colon,
    Preload,
    Not,
    Pi,
    Lambda,

    EndOfFile
}


public class Token(string val, TokenType type)
{
    public TokenType Type { get; } = type;
    public string StringValue { get; } = val;

    public int IntValue => int.Parse(StringValue);
    public bool BoolValue => bool.Parse(StringValue);
    public float FloatValue => float.Parse(StringValue, System.Globalization.CultureInfo.InvariantCulture);
    public double DoubleValue => double.Parse(StringValue, System.Globalization.CultureInfo.InvariantCulture);
    public byte ByteValue => byte.Parse(StringValue);

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

                _ => throw new ValidOperationException("Cannot find operator " + StringValue)
            };
        }
        
    }

    public override string ToString() => $"{nameof(Token)} {{ Value = {StringValue} | Type = {Type} }}";
}
