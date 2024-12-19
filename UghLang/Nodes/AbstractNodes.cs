namespace UghLang.Nodes;

/// <summary>
/// Creates faster implementation for expression node 
/// </summary>
/// <param name="expressionIndex">Expression node index</param>
public abstract class NestedExpressionNode(int expressionIndex = 0) : ASTNode
{
    protected ExpressionNode? exprs;
    public ExpressionNode Expression => exprs ?? throw new NullReferenceException("Null expression");

    public override void Load()
    {
        base.Load();
        exprs = GetNodeOrDefalut<ExpressionNode>(expressionIndex);
    }
}


/// <summary>
/// Creates faster implementation for expression and tag node 
/// </summary>
/// <param name="expressionIndex">Expression node index</param>
/// <param name="tagIndex">Tag node index</param>
public abstract class NestedExpressionAndTagNode(int expressionIndex = 0, int tagIndex = 1) : NestedExpressionNode(expressionIndex)
{
    protected TagNode? tag;
    public TagNode Tag => tag ?? throw new NullReferenceException("Null expression");

    public override void Load()
    {
        base.Load();
        tag = GetNodeOrDefalut<TagNode>(tagIndex);
    }
}