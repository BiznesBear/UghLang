namespace UghLang;

public enum Operator
{
    Equals,
    DoubleEquals,
    NotEquals,
    Plus,
    Minus,
    Multiply,
    Divide,
    PlusEquals,
    MinusEquals,
    SubtractEquals,
    MultiplyEquals,
    DivideEquals,
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
        switch (Operator)
        {
            case Operator.Equals: return Val2;
            case Operator.DoubleEquals: return Val1 == Val2;
            case Operator.NotEquals: return Val1 != Val2;
            case Operator.Plus or Operator.PlusEquals: return Val1 + Val2;
            case Operator.Minus or Operator.MinusEquals: return Val1 - Val2;
            case Operator.Multiply or Operator.MultiplyEquals: return Val1 * Val2;
            case Operator.Divide or Operator.DivideEquals: return Val1 / Val2;
            default: return Val1;
        }
    }

    public static Operator GetOperator(string opr)
    {
        return opr switch
        {
            "=" => Operator.Equals ,
            "==" => Operator.DoubleEquals,
            "!=" => Operator.NotEquals,
            "+" => Operator.Plus,
            "-" => Operator.Minus,
            "*" => Operator.Multiply,
            "/" => Operator.Divide,
            "+=" => Operator.PlusEquals,
            "-=" => Operator.SubtractEquals,
            "*=" => Operator.MultiplyEquals,
            "/=" => Operator.DivideEquals,
            _ => throw new("Cannot find operator " + opr),
        };
    }
}