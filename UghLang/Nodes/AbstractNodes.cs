namespace UghLang.Nodes;

/// <summary>
/// Assignes node by index 
/// </summary>
/// <typeparam name="T">Type of node</typeparam>
/// <param name="index">Index of node</param>
public abstract class AssignedNode<T>(int index = 0) : ASTNode 
{
    protected T? assigned;
    public T Assigned => assigned ?? throw new InvalidSpellingException(this);

    public override void Load()
    {
        base.Load();
        assigned = GetNodeOrDefalut<T>(index);
    }
}

/// <summary>
/// Creates faster implementation for expression and tag node 
/// </summary>
/// <param name="anyIndex">Expression node index</param>
/// <param name="tagIndex">Tag node index</param>
public abstract class AssignedIReturnAnyAndTagNode(int anyIndex = 0, int tagIndex = 1) : AssignedNode<IReturnAny>(anyIndex)
{
    protected TagNode? tag;
    public TagNode Tag => tag ?? throw new MissingThingException("{}", this);

    public override void Load()
    {
        base.Load();
        tag = GetNodeOrDefalut<TagNode>(tagIndex);
    }
}