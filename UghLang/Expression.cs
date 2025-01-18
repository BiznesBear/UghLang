using UghLang.Nodes;

namespace UghLang;




public static class Expression
{
    private static readonly Dictionary<Operator, int> OperatorPrecedence = new()
    {
        { Operator.Equals, 1 },
        { Operator.Lambda, 1 },
        { Operator.Or, 2 },
        { Operator.And, 3 },
        { Operator.DoubleEquals, 4 },
        { Operator.NotEquals, 4 },
        { Operator.Less, 5 },
        { Operator.Higher, 5 },
        { Operator.LessEquals, 5 },
        { Operator.HigherEquals, 5 },
        { Operator.Plus, 6 },
        { Operator.Minus, 6 },
        { Operator.Multiply, 7 },
        { Operator.Divide, 7 },
        { Operator.Power, 8 },
        { Operator.Sqrt, 9 },
    };


    public static object Express(IReadOnlyList<ASTNode> nodes) // TODO: Rework expressions
    {
        if (nodes.Count == 0) return 0;

        Stack<object> vals = new();
        Stack<Operator> operators = new();

        foreach (var node in nodes)
        {
            switch (node)
            {
                case IReturnAny d:
                    vals.Push(d.AnyValue);
                    break;
                case OperatorNode opNode:
                    while (operators.Count > 0 && GetPrecedence(operators.Peek()) >= GetPrecedence(opNode.Operator))
                        EvaluateOperation(vals, operators);
                    operators.Push(opNode.Operator);
                    break;
            }
        }

        while (vals.Count > 1)
            EvaluateOperation(vals, operators);

        return vals.First();
    }

    private static void EvaluateOperation(Stack<object> values, Stack<Operator> operators)
    {
        var right = values.Pop();
        var left = values.Pop();
        var op = operators.Count < 1 ? Operator.Multiply : operators.Pop();

        values.Push(Operate(left, right, op));
    }

    public static object Operate(dynamic left, dynamic right, Operator opr)
    {
        return opr switch
        {
            Operator.Equals or Operator.Lambda => right,
            Operator.Plus => left + right,
            Operator.Minus => left - right,
            Operator.Multiply => left * right,
            Operator.Divide => left / right,

            Operator.Power => Math.Pow(left, right),
            Operator.Sqrt => Math.Pow(left, 1f / right),

            // BOOLEAN
            Operator.NotEquals => left != right,
            Operator.DoubleEquals => left == right,
            Operator.Higher => left > right,
            Operator.Less => left < right,
            Operator.HigherEquals => left >= right,
            Operator.LessEquals => left <= right,
            Operator.And => left && right,
            Operator.Or => left || right,
            _ => throw new ArgumentException($"Unsupported operator: {opr}"),
        };
    }

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
            
            // OTHER
            "=>" => Operator.Lambda,

            _ => throw new UghException("Cannot find operator " + opr),
        };
    }

    public static bool IsOperator(this char c) => c == '=' || c == '+' || c == '-' || c == '*' || c == '/' || c == '<' || c == '>' || c == '!' || c == '&' || c == '|';
    public static int GetPrecedence(Operator opr) => OperatorPrecedence[opr];
}
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
    Or,
    Lambda
}