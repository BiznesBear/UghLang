namespace UghLang.Nodes;


public class OperatorNode : ASTNode
{
    public required Operator Operator { get; set; }
}

public class TagNode : ASTNode 
{
    public TagNode() => Executable = false;
}


public class ExpressionNode : ASTNode, IReturnAny
{
    public object AnyValue => Express();

    public object Express()
    {
        if (HasEmptyBranch()) throw new EmptyExpressionException(this);


        Stack<object> vals = new();
        Stack<Operator> operators = new();

        for (int i = 0; i < Nodes.Count; i++)
        {
            var node = Nodes[i];

            if (node is IReturnAny d)
                vals.Push(d.AnyValue);
            else if (node is OperatorNode opNode)
            {
                while (operators.Count > 0 &&
                       Operation.GetPrecedence(operators.Peek()) >= Operation.GetPrecedence(opNode.Operator))
                {
                    var right = vals.Pop();
                    var left = vals.Pop();
                    vals.Push(Operation.Operate(left, right, operators.Pop()));
                }
                operators.Push(opNode.Operator);
            }
        }

        while (vals.Count > 1)
        {
            if (operators.Count < 1) break;

            var left = vals.Pop();
            var right = vals.Pop();
            var op = operators.Pop();

            vals.Push(Operation.Operate(right, left, op)); // DONT ASK WHY right IS LEFT AND left IS RIGHT 
        }

        return vals.First();
    }
}
public class ListNode : ASTNode, IReturn<IReadOnlyList<object>> // TODO: Add geting from index
{
    public IReadOnlyList<object> Value { get; set; } = [];
    public object AnyValue => Value;


    public override void Load()
    {
        base.Load();
        Value = GetObjects().ToList();
    }

    private IEnumerable<object> GetObjects()
    {
        foreach(var node in GetNodes<IReturnAny>())
            yield return node.AnyValue;
    }

    public override string ToString() => string.Join(';', Value);
}

/// <summary>
/// Used to declare and set variables
/// </summary>
public class InitializeNode : ASTNode
{
    public required Token Token { get; init; }

    private IReturnAny? value;
    private OperatorNode? oprNode;
    private ExpressionNode? exprs;

    private bool isOperation;
    private Function? fun;
    private IEnumerable<IReturnAny> args = [];

    public override void Load()
    {
        base.Load();

        if (TryGetNode<OperatorNode>(0, out oprNode))
        {
            isOperation = true;
            value = GetNodes<IReturnAny>().First();
        }
        else if (TryGetNode<ExpressionNode>(0, out exprs) && Ugh.TryGetName(Token.StringValue, out Name n)) // TODO: Add recurrency here
        {
            fun = n.Get<Function>();
            args = exprs.GetNodes<IReturnAny>();
        }
        else throw new InvalidSpellingException(this);
    }

    public override void Execute()
    {
        base.Execute();

        if (isOperation && value is not null)
        {
            if (Ugh.TryGetName(Token.StringValue, out var variable) && oprNode is not null)
                variable.Value = Operation.Operate(variable.Value, value.AnyValue, oprNode.Operator);
            else Ugh.RegisterName(new Variable(Token.StringValue, value.AnyValue));
            return;
        }

        fun?.Invoke(args);
    }
}



public class NameNode : ASTNode, IReturnAny 
{
    public required Token Token { get; init; }
    public object AnyValue => GetName().AnyValue;
    public Name GetName() => Ugh.GetName(Token.StringValue);
}
