namespace UghLang;

public enum Operator
{
    Equals,
    NotEquals,
    Plus,
    Minus,
    Multiply,
    Divide,
    PlusEquals,
    MinusEquals,
    MultiplyEquals,
    DivideEquals,

    // BOOLEAN
    Less,
    Higher,
    DoubleEquals,
    LessEquals,
    HigherEquals,
}
public class Operation
{
    private Operator Operator { get; }
    private dynamic Val1 { get; }
    private dynamic Val2 { get; }

    public Operation(dynamic val1, dynamic val2, Operator opr) 
    {
        Operator = opr;
        Val1 = val1;
        Val2 = val2;
    }

    public dynamic GetResult()
    {
        return Operator switch
        {
            Operator.Equals => Val2,
            Operator.NotEquals => Val1 != Val2,
            Operator.Plus or Operator.PlusEquals => Val1 + Val2,
            Operator.Minus or Operator.MinusEquals => Val1 - Val2,
            Operator.Multiply or Operator.MultiplyEquals => Val1 * Val2,
            Operator.Divide or Operator.DivideEquals => Val1 / Val2,
            Operator.DoubleEquals => Val1 == Val2,
            Operator.Higher => Val1 > Val2,
            Operator.Less => Val1 < Val2,
            Operator.HigherEquals => Val1 >= Val2,
            Operator.LessEquals => Val1 <= Val2,
            _ => Val1,
        };
    }

    public static Operator GetOperator(string opr)
    {
        return opr switch
        {
            "=" => Operator.Equals ,
            "!=" => Operator.NotEquals,
            "+" => Operator.Plus,
            "-" => Operator.Minus,
            "*" => Operator.Multiply,
            "/" => Operator.Divide,
            "+=" => Operator.PlusEquals,
            "-=" => Operator.MinusEquals,
            "*=" => Operator.MultiplyEquals,
            "/=" => Operator.DivideEquals,
            "==" => Operator.DoubleEquals,
            "<" => Operator.Less,
            ">" => Operator.Higher,
            "<=" => Operator.LessEquals,
            ">=" => Operator.HigherEquals,
            _ => throw new("Cannot find operator " + opr),
        };
    }
}