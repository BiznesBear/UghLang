namespace UghLang;

public abstract class ASTNode
{
    public const ASTNode NULL = default;

    #region Properties
    public bool CanExecute { get; set; } = true;
    public int CurrentIteration { get; private set; }

    protected List<ASTNode> Nodes { get; } = new();

    private Ugh? ugh;
    public Ugh Ugh
    {
        get => ugh ?? throw new UghException("No Ugh in node");
        set => ugh = value;
    }

    private ASTNode? parent;
    public ASTNode Parent
    {
        get => parent ?? this;
        set => parent = value;
    }

    #endregion

    public void AddNode(ASTNode node)
    {
        node.Parent = this;
        node.Ugh = Ugh;
        Nodes.Add(node);
    }

    public bool HasEmptyBranch() => Nodes.Count < 1;

    #region GettingNodes


    public bool TryGetNextBrother<T>(out T node) where T : ASTNode
    {
        var index = Parent.CurrentIteration + 1;
        if (index < Parent.Nodes.Count )
        {
            var n = Parent.Nodes[index];
            if(n is T t)
            {
                node = t;
                return true;
            }
        }

        node = (T)NULL;
        return false;
    }

    public T? GetNodeWith<T>() where T : ASTNode
    {
        foreach (var n in Nodes)
        {
            if (CheckNodeType<T>(n))
                return (T)n;
        }
        return null;
    }

    public bool TryGetNodeWith<T>(out T node) where T : ASTNode
    {
        var n = GetNodeWith<T>();

        if(n is null)
        {
            node = (T)NULL;
            return false;
        }

        node = n;
        return true;
    }

    public IEnumerable<T> GetNodesWith<T>() 
    {
        foreach (var n in Nodes)
            if (n is T t) yield return t;
    }

    public IEnumerable<T> GetAllNodesWith<T>()
    {
        foreach (var n in Nodes)
        {
            if (n is T t) yield return t;
            foreach (var dyn in n.GetAllNodesWith<T>())
                yield return dyn;
        }
    }
    #endregion


    /// <summary>
    /// Used for preloading nodes (to avoid it in execution)
    /// </summary>
    public virtual void Load()
    { 
        for (int i = 0; i < Nodes.Count; i++)
            Nodes[i].Load();     
    }

    /// <summary>
    /// Execute all assigned nodes.
    /// </summary>
    public virtual void Execute()
    {
        for (int i = 0; i < Nodes.Count; i++) 
        {
            if (CanExecute)
            {
                CurrentIteration = i;
                Nodes[i].Execute();
            }
        }
    }

    public override string ToString() => $"{GetType().Name}";
    public static bool CheckNodeType<T>(ASTNode node) => node.GetType() == typeof(T);
}

#region OtherNodes
public class AST : ASTNode
{
    public AST(Ugh ugh)
    {
        Parent = this;
        Ugh = ugh;
    }
}

public class UndefinedNode : ASTNode {

    public override void Execute()
    {
        base.Execute();
        throw new NotImplementedException("Undefined instructions");
    }
}
#endregion

#region ValueNodes
public interface IReturnAny
{
    public object AnyValue { get; }
}
public interface IReturn<T> : IReturnAny
{
    public T Value { get; set; }
}
public class AnyValueNode<T>(T defalutValue) : ASTNode, IReturn<T>
{
    public T Value { get; set; } = defalutValue;
    public object AnyValue => Value ?? throw new UghException("Cannot find value");
}
public class StringValueNode : AnyValueNode<string> { public StringValueNode() : base(string.Empty) { } }
public class IntValueNode : AnyValueNode<int> { public IntValueNode() : base(0) { } }
public class BoolValueNode : AnyValueNode<bool> { public BoolValueNode() : base(false) { } }
public class FloatValueNode : AnyValueNode<float> { public FloatValueNode() : base(0f) { } }
#endregion

#region BasicNodes
public class OperatorNode : ASTNode
{
    public required Operator Operator { get; set; }
}

public class ExpressionNode : ASTNode, IReturnAny
{
    public object AnyValue => Express();

    public object Express()
    {
        if (HasEmptyBranch()) return Variable.NULL.Value;

        Stack<dynamic> vals = new();
        Stack<Operator> operators = new();

        foreach (var node in Nodes)
        {
            if (node is IReturnAny d)
                vals.Push(d.AnyValue);
            else if (node is OperatorNode opNode)
            {
                while (operators.Count > 0 &&
                       Operation.GetPrecedence(operators.Peek()) >= Operation.GetPrecedence(opNode.Operator))
                {
                    var right = vals.Pop();
                    var left = vals.Pop();
                    vals.Push(Operation.Operate(left, right, operators.Pop()));
                }
                operators.Push(opNode.Operator);
            }
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

public class TagNode : ASTNode
{
    public const TagNode EMPTY = default;

    public void BaseExecute() => base.Execute();
    public override void Execute() { return; }
}


/// <summary>
/// Used to declare and set variables
/// </summary>
public class InitializeNode : ASTNode
{
    public required Token Token { get; init; }

    private IReturnAny? value;
    private OperatorNode? oprNode;
    private ExpressionNode? argsNode;

    private bool isOperation;
    private Function? function;
    public override void Load()
    {
        base.Load();
        if(TryGetNodeWith<OperatorNode>(out var opr))
        {
            oprNode = opr;
            isOperation = true;
            return;
        }

        if(TryGetNodeWith<ExpressionNode>(out var arg))
        {
            argsNode = arg;
            if (Ugh.TryGetFunction(Token.StringValue, out Function fun))
                function = fun;
        }
    }

    public override void Execute() 
    {
        base.Execute();


        if (isOperation)
        {
            value = GetNodesWith<IReturnAny>().First();

            if (value is null) return; // TODO: Throw here exception

            if (Ugh.TryGetVariable(Token, out var variable) && oprNode is not null && isOperation)
                variable.Value = Operation.Operate(variable.Value, value.AnyValue, oprNode.Operator);
            else Ugh.InitializeVariable(new(Token.StringValue, value.AnyValue));
        }
        else if(function is not null) function.Invoke(); 
        else throw new UghException("Invalid initialize of object");
    }
}

/// <summary>
/// Used as refrence to variables by other nodes
/// </summary>
public class RefrenceNode : ASTNode, IReturnAny
{
    public required Token Token { get; init; }

    private object? val;
    public object AnyValue => val ?? throw new UghException();

    public override void Execute()
    {
        base.Execute();
        val = GetVariable().Value;
    }

    public Variable GetVariable()
    {
        Ugh.TryGetVariable(Token, out var variable);
        return variable;
    }
}
#endregion

#region KeywordNodes
public class PrintNode : ASTNode
{
    public override void Execute()
    {
        base.Execute();
        if (TryGetNodeWith<ExpressionNode>(out var expr))
            Console.WriteLine(expr.AnyValue);
        else throw new UghException();
    }
}
public class InputNode : AnyValueNode<string>
{
    public InputNode() : base(string.Empty) { }

    public override void Execute()
    {
        base.Execute();
        if (TryGetNodeWith<ExpressionNode>(out var expr))
            Console.Write(expr.AnyValue);
        Value = Console.ReadLine() ?? string.Empty;
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
        else if (TryGetNodeWith<AnyValueNode<string>>(out var modifier) && modifier.Value == "all")
            Ugh.FreeAllVariables();
        else throw new UghException();
    }
}
public class DeclareFuntionNode : ASTNode 
{
    private ExpressionNode? decArgument;
    private TagNode? tag;
    private RefrenceNode? name;

    public override void Load()
    {
        base.Load();
        TryGetNodeWith<ExpressionNode>(out var expr);
        TryGetNodeWith<TagNode>(out var t);
        TryGetNodeWith<RefrenceNode>(out var n);
        decArgument = expr;
        tag = t;
        name = n;

        if (decArgument is null) return;
        Function fun = new(name?.Token.StringValue ?? throw new UghException(), tag ?? TagNode.EMPTY);
        Ugh.DeclareFunction(fun);
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
/// Checks if next expression value equals true
/// </summary>
public class IfNode : ASTNode
{
    private ExpressionNode? exprs;
    private TagNode? tag;
    private ElseNode? elseNode; // TODO: Add implementation for else node

    public override void Load()
    {
        base.Load();

        TryGetNodeWith<ExpressionNode>(out var expr);
        exprs = expr;

        TryGetNodeWith<TagNode>(out var t);
        tag = t;

        if (TryGetNextBrother<ElseNode>(out var elN)) elseNode = elN;
    }

    public override void Execute()
    {
        base.Execute();
        if (exprs is not null)
        {
            if ((bool)exprs.AnyValue == true)
                tag?.BaseExecute();
            else elseNode?.BaseExecute();
        }
        else throw new UghException("Cannot find expression in if statement");
    }
}

/// <summary>
/// Else statment
/// </summary>
public class ElseNode : ASTNode
{
    private TagNode? tag;
    public override void Load()
    {
        base.Load();
        TryGetNodeWith<TagNode>(out var t);
        tag = t;
    }
    public void BaseExecute() => tag?.BaseExecute();
    public override void Execute() { return; }
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
            for (int i = 0; i < (int)exprs.AnyValue; i++)
                tag?.BaseExecute();
        }
    }
}
#endregion