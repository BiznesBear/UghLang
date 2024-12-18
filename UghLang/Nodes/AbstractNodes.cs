namespace UghLang.Nodes;


public abstract class NestedExpressionNode(int exIndx) : ASTNode
{
    private readonly int exprsIndex = exIndx;

    protected ExpressionNode? exprs;
    public ExpressionNode Expression => exprs ?? throw new NullReferenceException("Null expression");

    public NestedExpressionNode() : this(0) { }

    public override void Load()
    {
        base.Load();
        exprs = GetNode<ExpressionNode>(exprsIndex);
    }
}
public abstract class NestedExpressionAndTagNode(int exprIndx, int tgndx) : NestedExpressionNode(exprIndx)
{
    private readonly int tagIndex = tgndx;

    protected TagNode? tag;
    public TagNode Tag => tag ?? throw new NullReferenceException("Null expression");

    public NestedExpressionAndTagNode() : this(0, 1) { }

    public override void Load()
    {
        base.Load();
        tag = GetNode<TagNode>(tagIndex);
    }
}