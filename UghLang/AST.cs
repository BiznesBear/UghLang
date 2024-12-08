namespace UghLang;


public abstract class ASTNode
{
    public const ASTNode NULL = default;
    protected List<ASTNode> Nodes { get; } = new();

    private Ugh? ugh;
    public Ugh Ugh
    {
        get => ugh ?? throw new NullReferenceException("No assigned Ugh in node");
        set => ugh = value;
    }

    private ASTNode? parent;
    public ASTNode Parent
    {
        get => parent ?? this;
        set => parent = value;
    }

    /// <summary>
    /// Add node with assigned parent and ugh
    /// </summary>
    /// <param name="node">Node to assign</param>
    public void AddNode(ASTNode node)
    {
        node.Parent = this;
        node.Ugh = Ugh;
        Nodes.Add(node);
    }

    public bool TryGetNodeWithType<T>(out T node) where T : ASTNode
    {
        foreach (var n in Nodes)
        {
            if (CheckNodeType<T>(n))
            {
                node = (T)n;
                return true;
            }
        }
        node = (T)NULL;
        return false;
    }
    public IEnumerable<T> GetNodesWithType<T>() where T : ASTNode 
    {
        foreach (var n in Nodes)
            if (CheckNodeType<T>(n))
                yield return (T)n;
    }


    public static bool CheckNodeType<T>(ASTNode node)  => node.GetType() == typeof(T);


    /// <summary>
    /// Execute all assigned nodes.
    /// </summary>
    public virtual void Execute()
    {
        foreach (ASTNode node in Nodes)
        {
            Debug.Print("EXE " + node);
            node.Execute();
        }
    }
    public override string ToString() => $"{GetType().Name}";
}


public class AST : ASTNode
{
    public AST(Ugh ugh)
    {
        Parent = this;
        Ugh = ugh;
    }
}

public class UndefinedNode : ASTNode { }


/// <summary>
/// End value for node
/// </summary>
public class DynamicNode : ASTNode
{
    private dynamic? dyna;
    public virtual dynamic Dynamic { get => dyna ?? throw new NullReferenceException("DYNAMITE"); set => dyna = value; }
}

public class ExpressionNode : DynamicNode
{
    
}
public class PrintNode : ASTNode
{
    public override void Execute()
    {
        base.Execute();

        if(TryGetNodeWithType(out DynamicNode dyna)) Log(dyna.Dynamic);
        else if (TryGetNodeWithType(out RefrenceNode refr)) Log(refr.Dynamic);
    }
    private static void Log(object message) => Console.WriteLine(message);
}
public class FreeNode : ASTNode
{
    public override void Execute()
    {
        base.Execute();
        if (TryGetNodeWithType(out RefrenceNode node))
            Ugh.FreeVariable(node.GetVariable());
    }
}

/// <summary>
/// Used to declare and set variables
/// </summary>
public class InitVariableNode : ASTNode
{
    public required Token Token { get; init; }
    public required Operator Operator { get; init; }
    public required Token ValueToken { get; init; }

    public override void Execute()
    {
        base.Execute();
        if (Ugh.TryGetVariable(Token, out var variable))
            variable.Set(Operation.Operate(variable.Get(), ValueToken.Dynamic, Operator));
        else Ugh.DeclareVariable(new(Token.StringValue, ValueToken.Dynamic));
    }
}

/// <summary>
/// Used as refrence to variables by other nodes
/// </summary>
public class RefrenceNode : DynamicNode
{
    public required Token Token { get; init; }
    public override dynamic Dynamic { get => GetVariable().Get(); set => GetVariable().Set(value); }
    public Variable GetVariable()
    {
        if (Ugh.TryGetVariable(Token, out var variable))
            return variable;
        else throw new NullVariableRefrenceException(Token); 
    }
}
