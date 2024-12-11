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

    #region Searching
    public static bool CheckNodeType<T>(ASTNode node)  => node.GetType() == typeof(T);
    public bool HasEmptyBranch() => Nodes.Count < 1;
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
        {
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

public class TagNode : ASTNode
{
    public void BaseExecute() => base.Execute();
    public override void Execute() { return; }
}

/// <summary>
/// Contains result value for execution
/// </summary>
public class DynamicNode : ASTNode, IDynamic
{
    protected dynamic? dnmc;
    public dynamic Dynamic { get => dnmc ?? throw new NullReferenceException(); set => dnmc = value; }
}
public class OperatorNode : ASTNode
{
    public required Operator Operator { get; set; }
}

public class ExpressionNode : DynamicNode
{
    public override void Execute()
    {
        base.Execute();
        Dynamic = Express();
    }

    public dynamic Express()
    {
        if (HasEmptyBranch()) return Variable.NULL.Get();

        Stack<dynamic> vals = new();
        Stack<Operator> operators = new();

        foreach (var node in Nodes)
        {
            if (node is IDynamic d)
                vals.Push(d.Dynamic);
            else if (node is OperatorNode opNode)
                operators.Push(opNode.Operator);
        }

        while(vals.Count > 1)
        {
            if (operators.Count < 1) break;

            var left = vals.Pop();
            var right = vals.Pop();
            var op = operators.Pop();

            vals.Push(Operation.Operate(right, left, op)); // DONT ASK WHY right IS LEFT AND left IS RIGHT 
        }

        return vals.First();
    }
} 

/// <summary>
/// Used to declare and set variables
/// </summary>
public class InitVariableNode : ASTNode
{
    public required Token Token { get; init; }

    public override void Execute()
    {
        base.Execute();

        var val = GetChildrensWith<IDynamic>().First(); // TODO: Rework by creating here local expression
        
        if (Ugh.TryGetVariable(Token, out var variable) && TryGetNodeWith<OperatorNode>(out var opr))
            variable.Set(Operation.Operate(variable.Get(), val.Dynamic, opr.Operator));
        else Ugh.DeclareVariable(new(Token.StringValue, val.Dynamic));
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


public class PrintNode : ASTNode
{
    public override void Execute()
    {
        base.Execute();
        if (TryGetNodeWith<ExpressionNode>(out var expr))
            Console.WriteLine(expr.Dynamic);
        // TODO: Add exception here.
    }
}

public class FreeNode : ASTNode
{
    public override void Execute()
    {
        base.Execute();
        if (TryGetNodeWith(out RefrenceNode node))
            Ugh.FreeVariable(node.GetVariable());
        else if (TryGetNodeWith<DynamicNode>(out var modifier) && modifier.Dynamic == "all")
            Ugh.FreeAllVariables();
    }
}

/// <summary>
/// If statment
/// </summary>
public class IfNode : ASTNode
{
    public override void Execute()
    {
        base.Execute();
        if (TryGetNodeWith<ExpressionNode>(out var expr) && expr.Dynamic == true)
        {
            if (TryGetNodeWith<TagNode>(out var block))
            {
                block.BaseExecute();
            }
        }
    }
}
