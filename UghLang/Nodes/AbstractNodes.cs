namespace UghLang.Nodes;


public abstract class NestedExpressionNode(int exIndx) : ASTNode
{
    protected ExpressionNode? exprs;
    protected readonly int exprsIndex = exIndx;
    public NestedExpressionNode() : this(0) { }

    public override void Load()
    {
        base.Load();
        exprs = GetNode<ExpressionNode>(exprsIndex);
    }
}
public abstract class NestedExpressionAndTagNode(int exprIndx, int tgndx) : NestedExpressionNode(exprIndx)
{
    protected TagNode? tag;
    protected readonly int tagIndex = tgndx;

    public NestedExpressionAndTagNode() : this(0, 1) { }

    public override void Load()
    {
        base.Load();
        exprs = GetNode<ExpressionNode>(exprsIndex);
        tag = GetNode<TagNode>(tagIndex);
    }
}