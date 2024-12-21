namespace UghLang.Nodes;


/// <summary>
/// Creates faster implementation for IReturnAny node 
/// </summary>
/// <param name="index">Expression node index</param>
public abstract class AssignedIReturnAnyNode(int index = 0) : ASTNode
{
    protected IReturnAny? any;

    public override void Load()
    {
        base.Load();
        any = GetNodeOrDefalut<IReturnAny>(index);
    }

}

/// <summary>
/// Creates faster implementation for expression node 
/// </summary>
/// <param name="expressionIndex">Expression node index</param>
public abstract class AssignedExpressionNode(int expressionIndex = 0) : ASTNode
{
    protected ExpressionNode? exprs;
    public ExpressionNode Expression => exprs ?? throw new MissingThingException("()", this);


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
public abstract class AssignedExpressionAndTagNode(int expressionIndex = 0, int tagIndex = 1) : AssignedExpressionNode(expressionIndex)
{
    protected TagNode? tag;
    public TagNode Tag => tag ?? throw new MissingThingException("{}",this);

    public override void Load()
    {
        base.Load();
        tag = GetNodeOrDefalut<TagNode>(tagIndex);
    }
}