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
public record Pair(dynamic Left,dynamic Right,Operator Operator);
public static class Operation
{
    public static dynamic Operate(dynamic left, dynamic right, Operator opr)
    {
        return opr switch
        {
            Operator.Equals => right,
            Operator.NotEquals => left != right,
            Operator.Plus or Operator.PlusEquals => left + right,
            Operator.Minus or Operator.MinusEquals => left - right,
            Operator.Multiply or Operator.MultiplyEquals => left * right,
            Operator.Divide or Operator.DivideEquals => left / right,
            Operator.DoubleEquals => left == right,
            Operator.Higher => left > right,
            Operator.Less => left < right,
            Operator.HigherEquals => left >= right,
            Operator.LessEquals => left <= right,
            _ => left,
        };
    }
    public static dynamic Operate(this Pair pair) => Operate(pair.Left,pair.Right,pair.Operator);

    public static bool IsOperator(this char c) => c == '=' || c == '+' || c == '-' || c == '*' || c == '/' || c == '<' || c == '>';

    public static Operator GetOperator(this string opr)
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
