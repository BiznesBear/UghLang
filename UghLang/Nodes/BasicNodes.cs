namespace UghLang.Nodes;

public class EndOfFileNode : ASTNode;
public class OperatorNode : ASTNode, INextQuit
{
    public required Operator Operator { get; init; }
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
        LocalNames.ForEach(Rnm.FreeName);
        LocalNames.Clear();
    }
}

public class ExpressionNode : ASTNode, IReturn<object?>
{
    public object? Value { get; set; } = null;
    public object AnyValue => Value ?? expressionTree.Express();

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

public class NameNode : ASTNode, IReturnAny // TODO: ADD REQUESTS
{
    public required Token Token { get; init; }
    public object AnyValue => GetValue();
    public Name Name => name ?? Rnm.GetName(Token.StringValue);

    private Name? name;
    private IReturnAny? any;

    private OperatorNode? oprNode;
    private IndexNode? indexNode;
    private RefrenceNode? refrenceNode;
    private FunctionCallInfo? callInfo;

    public override void Load()
    {
        base.Load();

        if (Parent is INamingNode) 
            return;

        oprNode = FindNode<OperatorNode>();
        indexNode = FindNode<IndexNode>();
        refrenceNode = FindNode<RefrenceNode>();
        
        if (oprNode is not null) 
            any = oprNode.GetRight();
        else if (TryGetNode(0, out ExpressionNode exprsNode))
            callInfo = new FunctionCallInfo(exprsNode.GetArguments());
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
        
        if(name is null)
            Rnm.TryGetName(Token.StringValue, out name);
        if (refrenceNode is not null)
            name = refrenceNode.ReflectName(this);
        
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
            Rnm.RegisterName(v);

            if (Parent is BlockNode blockNode) // Add to local names if parent is block node
                blockNode.LocalNames.Add(v);
        }
    }
    
    private object GetValue() => indexNode is null ? Name.AnyValue : Name.GetAny<IList<object>>()![indexNode.Index];
}

public class OperableNodeNameNode : NameNode, IOperableNode;

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

public class NotNode : AssignedNode<IReturnAny>, IReturnAny, IOperableNode, INextQuit
{
    public object AnyValue => !Assigned.GetAny<bool>();
}

public class RefrenceNode : ASTNode, IOperableNode
{
    private NameNode? nameNode;
    public override void Load()
    {
        base.Load();
        nameNode = GetNode<NameNode>(0);
    }

    public Name ReflectName(NameNode parent)
    {
        var orginal = Rnm.GetName(parent.Token.StringValue);
        var nameSpace = orginal.Value as Namespace ?? throw new InvalidOperationException($"{orginal.Key} is not an object.");
        return nameSpace.GetName(nameNode!.Token.StringValue);
    } 
}