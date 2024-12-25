namespace UghLang.Nodes;


public class OperatorNode : ASTNode
{
    public required Operator Operator { get; init; }
}

public class TagNode : ASTNode 
{
    public event Action? EndedExecution;
    public TagNode() => Executable = false;
    public override void Execute()
    {
        base.Execute();
        EndedExecution?.Invoke();
    }
}


public class ExpressionNode : ASTNode, IReturnAny
{
    public object AnyValue => Express();

    private object Express()
    {
        if (HasEmptyBranch()) throw new EmptyExpressionException(this);


        Stack<object> vals = new();
        Stack<Operator> operators = new();
        
        foreach(var node in Nodes)
        {
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

            var right = vals.Pop();
            var left = vals.Pop();
            var op = operators.Pop();

            vals.Push(Operation.Operate(left, right, op));
        }

        while (vals.Count > 1)
        {
            var right = vals.Pop();
            var left = vals.Pop();
            
            vals.Push(Operation.Operate(left, right, Operator.Multiply));
        }
        
        return vals.First();
    }
}
public class ListNode : AssignedNode<ConstIntValueNode>, IReturnAny
{
    public object AnyValue => new object[Assigned.Value];
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
    private ListNode? list;
    
    private BaseFunction? fun;
    private IEnumerable<IReturnAny> args = [];

    public override void Load()
    {
        base.Load();

        if (TryGetNode<OperatorNode>(0, out oprNode))
            value =  GetNode<IReturnAny>(1);
        else if (TryGetNode<ExpressionNode>(0, out exprs) && Ugh.TryGetName(Token.StringValue, out Name n)) 
        {
            fun = n.Get<BaseFunction>();
            args = exprs.GetNodes<IReturnAny>();
        }
        else if (TryGetNode<ListNode>(0, out list) && TryGetNode<OperatorNode>(1, out oprNode))
            value = GetNode<IReturnAny>(2);
        else throw new InvalidSpellingException(this);
    }

    public override void Execute()
    {
        base.Execute();

        if (oprNode is not null && value is not null)
        {
            if (Ugh.TryGetName(Token.StringValue, out var variable))
            {
                if (list is not null)
                {
                    var array = variable.Value as object[];
                    array![list.Assigned.Value] = Operation.Operate(array[list.Assigned.Value], value.AnyValue, oprNode.Operator);
                }
                else
                {
                    variable.Value = Operation.Operate(variable.Value, value.AnyValue, oprNode.Operator);
                }
            }
            else
            {
                var v = new Variable(Token.StringValue, value.AnyValue);
                Ugh.RegisterName(v);

                if(Parent is TagNode tgn) // deregister variable after end of tag node execution
                    tgn.EndedExecution += () => { Ugh.FreeName(v); };
            } 
            return;
        }

        fun?.Invoke(args);
    }
}


public class NameNode : AssignedNode<ExpressionNode>, IReturnAny, IOperatable
{
    public required Token Token { get; init; }
    public object AnyValue => GetValue(); // TODO: Make optimalizations for this

    private BaseFunction? fun;
    private IEnumerable<IReturnAny> args = [];
    private ListNode? list;

    public Name GetName() => Ugh.GetName(Token.StringValue);
    
    public object GetValue()
    {
        var name = GetName();
        if (list is null) return name.AnyValue;
        
        var array = name.AnyValue as object[];
        return array![list.Assigned.Value];
    }

    public override void Load()
    {
        base.Load();
        
        list = GetNodeOrDefalut<ListNode>(0);
        if (Parent.CheckType<INamed>() || assigned is null) return;
        
        fun = GetName().Get<BaseFunction>();
        args = assigned.GetNodes<IReturnAny>();
    }

    public override void Execute()
    {
        base.Execute();
        fun?.Invoke(args);
    }

}
