namespace UghLang.Nodes;


public class OperatorNode : ASTNode
{
    public required Operator Operator { get; init; }
}

public class BlockNode : ASTNode, IReturnAny
{
    public BlockNode() => Executable = false;
    public List<Name> LocalNames { get; } = new();

    public object AnyValue => ArrayNode.NodesToArray(Nodes);

    public void FreeLocalNames()
    {
        LocalNames.ForEach(Ugh.FreeName);
        LocalNames.Clear();
    }
}

public class ExpressionNode : ASTNode, IReturnAny
{
    public object AnyValue => Express();
    private object Express() => Operation.Express(Nodes);
}

public class ArrayNode : AssignedNode<IReturnAny>, IReturnAny
{
    public int Index
    {
        get
        {
            try { return (int)Assigned.AnyValue; }
            catch (Exception ex) { Debug.Error(ex); }
            return 0;
        }
    }

    public object AnyValue => NodesToArray(Nodes);
    public static object[] NodesToArray(IReadOnlyList<ASTNode> nodes) => nodes.OfType<IReturnAny>().Select(a => a.AnyValue).ToArray();
}


/// <summary>
/// Used to declare and set variables
/// </summary>

public class NameNode : ASTNode, IReturnAny 
{
    public required Token Token { get; init; }

    public object AnyValue => GetValue();
    public Name Name => name ?? throw new UghException("Cannot find name: " + Token.StringValue);

    private Name? name;


    private IReturnAny? any;
    private OperatorNode? oprNode;
    private ArrayNode? arrayNode;
    
    private BaseFunction? fun;
    private IEnumerable<IReturnAny> args = [];
    
    public override void Load()
    {
        base.Load();

        bool n = Ugh.TryGetName(Token.StringValue, out name);

        if (Parent.CheckType<INamed>()) return;

        if (TryGetNode<OperatorNode>(0, out oprNode))
            any =  HandleGetNode<IReturnAny>(1);
        else if (TryGetNode<ExpressionNode>(0, out var exprsNode) && n) 
        {
            fun = name.GetAs<BaseFunction>();
            args = exprsNode.GetNodes<IReturnAny>();
        }
        else if (TryGetNode<ArrayNode>(0, out arrayNode) && TryGetNode<OperatorNode>(1, out oprNode))
            any = HandleGetNode<IReturnAny>(2);
    }

    public override void Execute()
    {
        base.Execute();
        
        if (oprNode is null || any is null)
        {
            fun?.Invoke(args);
            return;
        }

        if (name is null) Ugh.TryGetName(Token.StringValue, out name);
        
        if(name is not null)
        {
            if (arrayNode is not null)
            {
                var array = name.Value as IList<object>;
                array![arrayNode.Index] = Operation.Operate(array[arrayNode.Index], any.AnyValue, oprNode.Operator);
            }
            else
                name.Value = Operation.Operate(name.Value, any.AnyValue, oprNode.Operator);
        }

        else // Initialization
        {
            var v = Parent is ConstNode? new Constant(Token.StringValue, any.AnyValue) : new Variable(Token.StringValue, any.AnyValue);
            Ugh.RegisterName(v);
            name = v;
            if (Parent is BlockNode blockNode) // deregister variable after end of tag node execution
                blockNode.LocalNames.Add(v);
        }
    }
    
    public object GetValue()
    {
        if (arrayNode is null) return Name.AnyValue;
        
        var array = Name.AnyValue as IList<object>;
        return array![arrayNode.Index];
    }
}

public class OperableNameNode : NameNode, IOperable;
