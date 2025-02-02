using UghLang.Nodes;
namespace UghLang;



public readonly struct ExpressionTree // TODO: add truncation for constant values
{
    private readonly IReturnAny? Last { get; }
    public ExpressionTree(ASTNode node)
    {
        var values = new Stack<IReturnAny>();  
        var operators = new Stack<OperatorNode>(); 

        var first = node.GetNodeOrDefalut<IReturnAny>(0);
        var oprNodes = node.GetNodes<OperatorNode>();

        if(first is null) return;
        else if (!oprNodes.Any())
        {
            Last = first;
            return;
        }

        values.Push(first);


        foreach (var op in oprNodes)
        {
            while (operators.Count > 0 && BinaryOperation.OperatorPrecedence[operators.Peek().Operator] >= BinaryOperation.OperatorPrecedence[op.Operator])
            {
                var right = values.Pop();
                var left = values.Pop();
                var operatorNode = operators.Pop();

                values.Push(new BinaryOperation(left, right, operatorNode.Operator)); 
            }
            operators.Push(op); 
            values.Push(op.GetRight());
        }


        while (operators.Count > 0)
            EvaluateOperation();

        void EvaluateOperation()
        {
            var right = values.Pop();
            var left = values.Pop();
            var op = operators.Pop().Operator;

            values.Push(new BinaryOperation(left, right, op));
        }

        Last = values.Pop();
    }

    public readonly object Express() => Last?.AnyValue ?? null!;
}

public enum Operator
{
    Equals,
    Plus,
    Minus,
    Multiply,
    Divide,
    DivideRest,
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
}
public class BinaryOperation(IReturnAny left, IReturnAny right,Operator op) : IReturnAny
{
    public object AnyValue => Operate();
    public Operator Operator { get; set; } = op;
    public IReturnAny Left { get=> left ?? throw new NullReferenceException(); set=> left = value; } 
    public IReturnAny Right { get => right ?? throw new NullReferenceException(); set => right = value; } 
    

    private IReturnAny? left = left;
    private IReturnAny? right = right;

    public object Operate() => Operate(Left.AnyValue, Right.AnyValue, Operator);
    public static object Operate(object left, object right, Operator opr) => OperateType<object>(left, right, opr);
    public static T OperateType<T>(dynamic left, dynamic right, Operator opr)
    {
        return opr switch
        {
            Operator.Equals => right,
            Operator.Plus => left + right,
            Operator.Minus => left - right,
            Operator.Multiply => left * right,
            Operator.Divide => left / right,
            Operator.DivideRest => left % right,

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

    public static bool IsOperator(char c) => c == '=' || c == '+' || c == '-' || c == '*' || c == '/' || c == '<' || c == '>' || c == '!' || c == '&' || c == '|' || c == '%';

    private static readonly Dictionary<Operator, int> operatorPrecedence = new()
    {
        { Operator.Equals, 1 },
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
        { Operator.DivideRest, 7 },
        { Operator.Power, 8 },
        { Operator.Sqrt, 9 },
    };

    public static IReadOnlyDictionary<Operator, int> OperatorPrecedence => operatorPrecedence;
    public override string ToString() => $"BinOpr({Operator}, {Left.AnyValue}, {Right.AnyValue})";
}