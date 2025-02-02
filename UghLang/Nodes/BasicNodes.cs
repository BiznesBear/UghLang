namespace UghLang.Nodes;

public class EndOfFileNode : ASTNode;
public class OperatorNode : ASTNode, IInstantQuit
{
    public required Operator Operator { get; init; }
    public IReturnAny GetRight() => GetNodeOrDefalut<IReturnAny>(0) ?? throw new InvalidSpellingException(this,": cannot find right side of operation");
}


public class BlockNode : ASTNode, IReturnAny
{
    public BlockNode() => Executable = false;

    public List<Name> LocalNames { get; } = new();
    public object AnyValue => IndexNode.NodesToArray(Nodes);

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
    public override void Load() 
    {
        base.Load();
        expressionTree = new ExpressionTree(this);
    }
}

public class IndexNode : AssignedNode<IReturnAny>, IReturnAny
{
    public int Index => (int)Assigned.AnyValue;
    public object AnyValue => new object[Index];
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
    private IndexNode? arrayNode;
    private BaseFunction? fun;
    private IEnumerable<IReturnAny> args = [];
    private bool exeAsFun;


    public override void Load()
    {
        base.Load();

        if (Parent is INaming) return;

        if (TryGetNode<OperatorNode>(0, out oprNode)) any = oprNode.GetRight();
        else if (TryGetNode(0, out ExpressionNode exprsNode))
        {
            args = args.Concat(exprsNode.GetNodes<IReturnAny>());
            exeAsFun = true;
        }
        else if (TryGetNode<IndexNode>(0, out arrayNode) && TryGetNode<OperatorNode>(1, out oprNode))
            any = oprNode.GetRight();
    }

    public override void Execute()
    {
        base.Execute();

        if (name is null) Ugh.TryGetName(Token.StringValue, out name);

        if (exeAsFun)
        {
            fun ??= name.GetAs<BaseFunction>();
            fun.Invoke(args);
            return;
        }

        if (any is null || oprNode is null) return;

        if (name is not null)
        {
            if (arrayNode is not null)
                (name.Value as IList<object>)![arrayNode.Index] = BinaryOperation.Operate(name.GetAny<IList<object>>()[arrayNode.Index], any.AnyValue, oprNode.Operator);
            else name.Value = BinaryOperation.Operate(name.Value, any.AnyValue, oprNode.Operator);
        }
        else
        {
            var v = new Variable(Token.StringValue, any.AnyValue);
            Ugh.RegisterName(v);
            name = v;

            if (Parent is BlockNode blockNode) blockNode.LocalNames.Add(v);
        }
    }

    public object GetValue() => arrayNode is null ? Name.AnyValue : Name.GetAny<IList<object>>()![arrayNode.Index];
    public void SetArgs(IReturnAny arg) => args = [arg];
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