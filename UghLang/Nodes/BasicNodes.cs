namespace UghLang.Nodes;

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

        while (vals.Count > 1)
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
/// Does nothing on execution, but can be executed with BaseExecute
/// </summary>
public class TagNode : ASTNode
{
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
        if (TryGetNodeWith<OperatorNode>(out var opr))
        {
            oprNode = opr;
            isOperation = true;
            return;
        }

        if (TryGetNodeWith<ExpressionNode>(out var arg))
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
        else if (function is not null) function.Invoke();
        else throw new UghException("Invalid initialize of object");
    }
}


public class NameNode : ASTNode, IReturnAny // TODO: Rework this
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