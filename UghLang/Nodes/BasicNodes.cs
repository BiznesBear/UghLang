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
        if (HasEmptyBranch())
        {
            Debug.Warring("Tried calculate empty expression");
            return 0; 
        }

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
            var right = vals.Pop();
            var left = vals.Pop();
            var op = operators.Count < 1? Operator.Multiply : operators.Pop();

            vals.Push(Operation.Operate(left, right, op));
        }
        
        return vals.First();
    }
}
public class ListNode : AssignedNode<IReturnAny>, IReturn<object[]>
{
    public int Index
    {
        get
        {
            try { return (int)Assigned.AnyValue; }
            catch(Exception ex) { Debug.Error(ex.Message); }
            return 0;
        }
    }
        
    public object AnyValue => new object[Index];
    
    public object[] Value { get; set; } = Array.Empty<object>();
}


/// <summary>
/// Used to declare and set variables
/// </summary>
public class InitializeNode : ASTNode
{
    public required Token Token { get; init; }

    private IReturnAny? any;
    private OperatorNode? oprNode;
    private ExpressionNode? exprs;
    private ListNode? list;
    
    private BaseFunction? fun;
    private IEnumerable<IReturnAny> args = [];

    public override void Load()
    {
        base.Load();

        if (TryGetNode<OperatorNode>(0, out oprNode))
            any =  HandleGetNode<IReturnAny>(1);
        else if (TryGetNode<ExpressionNode>(0, out exprs) && Ugh.TryGetName(Token.StringValue, out Name n)) 
        {
            fun = n.GetAs<BaseFunction>();
            args = exprs.GetNodes<IReturnAny>();
        }
        else if (TryGetNode<ListNode>(0, out list) && TryGetNode<OperatorNode>(1, out oprNode))
        {
            any = HandleGetNode<IReturnAny>(2);
        }
        else throw new InvalidSpellingException(this);
    }

    public override void Execute()
    {
        base.Execute();
        
        if (oprNode is null || any is null)
        {
            fun?.Invoke(args);
            return;
        }

        if (Ugh.TryGetName(Token.StringValue, out var variable))
        {
            if (list is not null)
            {
                var array = variable.Value as object[];
                array![list.Index] = Operation.Operate(array[list.Index], any.AnyValue, oprNode.Operator);
            }
            else
                variable.Value = Operation.Operate(variable.Value, any.AnyValue, oprNode.Operator);
        }
        else // Initialization
        {
            var v = new Variable(Token.StringValue, any.AnyValue);
            Ugh.RegisterName(v);

            if(Parent is TagNode tgn) // deregister variable after end of tag node execution
                tgn.EndedExecution += () => { Ugh.FreeName(v); };
        }
    }
}


public class NameNode : AssignedNode<ExpressionNode>, IReturnAny, IOperable
{
    public required Token Token { get; init; }
    public object AnyValue => GetValue(); // TODO: Make optimizations for this

    private BaseFunction? fun;
    private IEnumerable<IReturnAny> args = [];
    private ListNode? list;

    public Name GetName() => Ugh.GetName(Token.StringValue);
    
    public object GetValue()
    {
        var name = GetName();
        if (list is null) return name.AnyValue;
        
        var array = name.AnyValue as object[];
        return array![list.Index];
    }

    public override void Load()
    {
        base.Load();
        
        list = GetNodeOrDefalut<ListNode>(0);
        if (Parent.CheckType<INamed>() || assigned is null) return;
        
        fun = GetName().GetAs<BaseFunction>();
        args = assigned.GetNodes<IReturnAny>();
    }

    public override void Execute()
    {
        base.Execute();
        fun?.Invoke(args);
    }

}
