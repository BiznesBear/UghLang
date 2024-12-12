namespace UghLang;


public abstract class ASTNode
{
    public const ASTNode NULL = default;

    #region Properties
    public bool CanExecute { get; set; } = true;
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
    /// Used for preloading nodes (to avoid it in execution)
    /// </summary>
    public virtual void Load()
    {
        foreach (ASTNode node in Nodes)
            node.Load();
    }

    /// <summary>
    /// Execute all assigned nodes.
    /// </summary>
    public virtual void Execute()
    {
        foreach (ASTNode node in Nodes)
        {
            if (!CanExecute) return;
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
public class ValueNode : ASTNode, IValue
{
    protected dynamic? dnmc;
    public dynamic Value { get => dnmc ?? throw new NullReferenceException(); set => dnmc = value; }
}

public class OperatorNode : ASTNode
{
    public required Operator Operator { get; set; }
}

public class ExpressionNode : ValueNode
{
    public override void Execute()
    {
        base.Execute();
        Value = Express();
    }

    public dynamic Express()
    {
        if (HasEmptyBranch()) return Variable.NULL.Get();

        Stack<dynamic> vals = new();
        Stack<Operator> operators = new();

        foreach (var node in Nodes)
        {
            if (node is IValue d)
                vals.Push(d.Value);
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
    private IValue? value;
    private bool isSet;
    private OperatorNode? oprNode;

    public override void Load()
    {
        base.Load();
        isSet = TryGetNodeWith<OperatorNode>(out var opr);
        oprNode = opr;
    }

    public override void Execute() 
    {
        base.Execute();

        value = GetChildrensWith<IValue>().First();

        if (value is null) return; // TODO: Throw here exception

        if (Ugh.TryGetVariable(Token, out var variable) && oprNode is not null && isSet)
            variable.Set(Operation.Operate(variable.Get(), value.Value, oprNode.Operator));
        else Ugh.DeclareVariable(new(Token.StringValue, value.Value));
    }
}

/// <summary>
/// Used as refrence to variables by other nodes
/// </summary>
public class RefrenceNode : ValueNode
{
    public required Token Token { get; init; }

    public override void Execute()
    {
        base.Execute();
        Value = GetVariable().Get();
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
            Console.WriteLine(expr.Value);
        // TODO: Add exception here.
    }
}

public class FreeNode : ASTNode
{
    private bool isRef;
    private RefrenceNode? refNode;
    public override void Load()
    {
        base.Load();
        isRef = TryGetNodeWith(out RefrenceNode refr);
        refNode = refr;
    }

    public override void Execute()
    {
        base.Execute();
        if (isRef && refNode is not null)
            Ugh.FreeVariable(refNode.GetVariable());
        else if (TryGetNodeWith<ValueNode>(out var modifier) && modifier.Value == "all")
            Ugh.FreeAllVariables();
        return; // TODO: Throw here exception
    }
}

public class BreakNode : ASTNode
{
    public override void Execute()
    {
        base.Execute();
        Parent.CanExecute = false;
    }
}

/// <summary>
/// If statment
/// </summary>
public class IfNode : ASTNode
{
    private ExpressionNode? exprs;
    private TagNode? tag;
    public override void Load()
    {
        base.Load();
        TryGetNodeWith<ExpressionNode>(out var expr);
        TryGetNodeWith<TagNode>(out var t);
        exprs = expr;
        tag = t;
    }
    public override void Execute()
    {
        base.Execute();
        if (exprs is not null && exprs.Value == true)
            tag?.BaseExecute();
    }
}

/// <summary>
/// While statment
/// </summary>
public class RepeatNode : ASTNode
{
    private ExpressionNode? exprs;
    private TagNode? tag;
    public override void Load()
    {
        base.Load();
        TryGetNodeWith<ExpressionNode>(out var expr);
        TryGetNodeWith<TagNode>(out var t);
        exprs = expr;
        tag = t;
    }

    public override void Execute()
    {
        base.Execute();
        if (exprs is not null)
        {
            for (int i = 0; i < exprs.Value; i++)
                tag?.BaseExecute();
        }
    }
}
