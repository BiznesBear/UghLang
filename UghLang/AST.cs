namespace UghLang;


public abstract class ASTNode
{
    public const ASTNode NULL = default;

    #region Properties
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
    #endregion

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

    #region Generics
    public static bool CheckNodeType<T>(ASTNode node)  => node.GetType() == typeof(T);

    public bool TryGetNodeWith<T>(out T node) where T : ASTNode
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
    /// <summary>
    /// Searches for T in Nodes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IEnumerable<T> GetChildrensWith<T>() 
    {
        foreach (var n in Nodes)
            if (n is T t) yield return t;
    }

    /// <summary>
    /// Searches for T in Nodes and nodes of childrens
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public IEnumerable<T> GetAllChildrensWith<T>()
    {
        foreach (var n in Nodes)
        {
            if (n is T t) yield return t;
            foreach (var dyn in n.GetChildrensWith<T>())
                yield return dyn;
        }
    }
    #endregion

    /// <summary>
    /// Execute all assigned nodes.
    /// </summary>
    public virtual void Execute()
    {
        foreach (ASTNode node in Nodes)
            node.Execute();
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
public class DynamicNode : ASTNode, IDynamic
{
    protected dynamic? dnmc;
    public dynamic Dynamic { get => dnmc ?? throw new NullReferenceException(); set => dnmc = value; }
}

public class ExpressionNode : DynamicNode
{
    
}
public class PrintNode : ASTNode
{
    private static void Log(object message) => Console.Write(message);
    public override void Execute()
    {
        base.Execute();
        if(TryGetNodeWith<ExpressionNode>(out var expr))
        {
            foreach(var obj in expr.GetChildrensWith<IDynamic>())
                Log(obj.Dynamic);
            Log('\n');
        }
    }
}
public class FreeNode : ASTNode
{
    public override void Execute()
    {
        base.Execute();
        if (TryGetNodeWith(out RefrenceNode node))
            Ugh.FreeVariable(node.GetVariable());
        else if(TryGetNodeWith<DynamicNode>(out var modifier) && modifier.Dynamic == "all")
            Ugh.FreeAllVariables();
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

    public override void Execute()
    {
        base.Execute();
        Dynamic = GetVariable().Get();
    }

    public Variable GetVariable()
    {
        if (Ugh.TryGetVariable(Token, out var variable))
            return variable;
        else throw new NullVariableRefrenceException(Token); 
    }
}
