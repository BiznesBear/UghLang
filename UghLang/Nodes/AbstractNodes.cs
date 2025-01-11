using System.ComponentModel.Design.Serialization;

namespace UghLang.Nodes;

/// <summary>
/// Assignes node
/// </summary>
/// <param name="index">Index of assigned node</param>
/// <typeparam name="T">Type of node</typeparam>
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
/// Assigns TagNode and IReturnAny nodes   
/// </summary>
/// <param name="anyIndex">Index for IReturnAny node</param>
/// <param name="tagIndex">Index for TagNode node</param>
public abstract class AssignedIReturnAnyAndTagNode(int anyIndex = 0, int tagIndex = 1) : AssignedNode<IReturnAny>(anyIndex)
{
    protected BlockNode? tag;
    public BlockNode Block => tag ?? throw new MissingException("{}", this);

    public override void Load()
    {
        base.Load();
        tag = GetNodeOrDefalut<BlockNode>(tagIndex);
    }
}

public abstract class TagNode : ASTNode
{
    protected abstract bool State { get; }
    public override void Load()
    {
        if (State)
        {
            Executable = false;
            return;
        }

        if (TryGetNode<BlockNode>(0, out var tag)) // Nested local
            tag.Executable = true;

        base.Load();
    }
}