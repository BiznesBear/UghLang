namespace UghLang.Nodes;

public class EndOfFileNode : ASTNode;
public class OperatorNode : ASTNode, IInstantQuit
{
    public required Operator Operator { get; init; }
    public IReturnAny GetLeft() => GetBrother<IReturnAny>(-1) ?? throw new InvalidSpellingException(this,": cannot find left side of operation");
    public IReturnAny GetRight() => GetNodeOrDefalut<IReturnAny>(0) ?? throw new InvalidSpellingException(this,": cannot find right side of operation");
}


public class BlockNode : ASTNode, IReturn<object[]>
{
    public BlockNode() => Executable = false;
    public List<Name> LocalNames { get; } = new();
    
    public object AnyValue => Value;
    public object[] Value => IndexNode.NodesToArray(Nodes);

    public void FreeLocalNames()
    {
        LocalNames.ForEach(Ugh.FreeName);
        LocalNames.Clear();
    }
}

public class ExpressionNode : ASTNode, IReturn<object?>
{
    public object? Value { get; set; } = null;
    public object AnyValue => Value is not null? Value : expressionTree.Express();

    private ExpressionTree expressionTree;
    public IReturnAny[] GetArguments() => GetNodes<IReturnAny>().ToArray() ?? throw new ValidOperationException("null arguments in expression");

    public override void Load() 
    {
        base.Load();
        expressionTree = new ExpressionTree(this);
    }
}

public class IndexNode : AssignedNode<IReturnAny>, IReturnAny
{
    public int Index => (int)Assigned.AnyValue;
    public object AnyValue
    {
        get
        {
            var qinit = GetBrother<BlockNode>()?.Value ?? [];

            if (qinit is object[] existingArray)
            {
                var newArray = new object[Index];

                Array.Copy(existingArray, newArray, Math.Min(existingArray.Length, Index));

                return newArray;
            }

            return Enumerable.Repeat(qinit, Index).ToArray();
        }
    } 

    public static object[] NodesToArray(IReadOnlyList<ASTNode> nodes) => nodes.OfType<IReturnAny>().Select(a => a.AnyValue).ToArray();
}

public class NameNode : ASTNode, IReturnAny
{
    public required Token Token { get; init; }
    public object AnyValue => GetValue();
    public Name Name => name ?? Ugh.GetName(Token.StringValue);

    private Name? name;
    private IReturnAny? any;

    private OperatorNode? oprNode;
    private IndexNode? indexNode;
    private FunctionCallInfo? callInfo;

    public override void Load()
    {
        base.Load();

        if (Parent is INaming) return;

        if (TryGetNode<OperatorNode>(0, out oprNode) || (TryGetNode<IndexNode>(0, out indexNode) && TryGetNode<OperatorNode>(1, out oprNode))) 
            any = oprNode.GetRight();
        else if (TryGetNode(0, out ExpressionNode exprsNode))
            callInfo = new(exprsNode.GetArguments());
    }

    public override void Execute()
    {
        base.Execute();

        if (callInfo is not null)
        {
            callInfo.Function ??= Name.GetAs<BaseFunction>();
            callInfo.Invoke();
            return;
        }

        if (any is null || oprNode is null) // Get
            return;

        Ugh.TryGetName(Token.StringValue, out name);

        if (name is not null) // Set
        {
            // Operate on array item
            if (indexNode is not null)
                (name.Value as IList<object>)![indexNode.Index] = BinaryOperation.Operate(name.GetAny<IList<object>>()[indexNode.Index], any.AnyValue, oprNode.Operator);

            else  // Normal operation
                name.Value = BinaryOperation.Operate(name.Value, any.AnyValue, oprNode.Operator);
        }
        else // Register variable
        {
            var v = new Variable(Token.StringValue, any.AnyValue);
            name = v;
            Ugh.RegisterName(v);

            if (Parent is BlockNode blockNode) // Add to local names if parent is block node
                blockNode.LocalNames.Add(v);
        }
    }

    /// <summary>
    /// Gets real value of the name
    /// </summary>
    /// <returns>Name value</returns>
    public object GetValue() => indexNode is null ? Name.AnyValue : Name.GetAny<IList<object>>()![indexNode.Index];
}

public class OperableNameNode : NameNode, IOperable;

public class PreloadNode : ASTNode
{
    public override void Load()
    {
        if (TryGetNode<BlockNode>(0, out var tag)) // Nested preload
            tag.Executable = true;

        base.Load();
        base.Execute();
    }
    public override void Execute() { }
}

public class NotNode : AssignedNode<IReturnAny>, IReturnAny, IOperable, IInstantQuit
{
    public object AnyValue => Assigned.GetAny<bool>();
}