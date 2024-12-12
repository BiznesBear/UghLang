namespace UghLang;

public enum Operator
{
    Equals,
    Plus,
    Minus,
    Multiply,
    Divide,
    Power,
    Sqrt,

    // BOOLEAN
    NotEquals,
    Less,
    Higher,
    DoubleEquals,
    LessEquals,
    HigherEquals,
    And,
    Or
}
public static class Operation
{
    public static dynamic Operate(dynamic left, dynamic right, Operator opr)
    {
        return opr switch
        {
            Operator.Equals => right,
            Operator.Plus => left + right,
            Operator.Minus => left - right,
            Operator.Multiply => left * right,
            Operator.Divide => left / right,

            Operator.Power => Math.Pow(right, left),
            Operator.Sqrt => Math.Pow(right, 1f / left),

            // BOOLEAN
            Operator.NotEquals => left != right,
            Operator.DoubleEquals => left == right,
            Operator.Higher => left > right,
            Operator.Less => left < right,
            Operator.HigherEquals => left >= right,
            Operator.LessEquals => left <= right,
            Operator.And => left && right,
            Operator.Or => left || right,
            _ => left,
        };
    }
    public static bool IsOperator(this char c) => c == '=' || c == '+' || c == '-' || c == '*' || c == '/' || c == '<' || c == '>' || c == '!' || c == '&' || c == '|';

    public static Operator GetOperator(this string opr)
    {
        return opr switch
        {
            "=" => Operator.Equals ,
            "+" or "+=" => Operator.Plus,
            "-" or "-=" => Operator.Minus,
            "*" or "*=" => Operator.Multiply,
            "/" or "/=" => Operator.Divide,

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

            _ => throw new("Cannot find operator " + opr),
        };
    }
}
