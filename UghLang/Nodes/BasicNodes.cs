﻿namespace UghLang.Nodes;


public class OperatorNode : ASTNode
{
    public required Operator Operator { get; init; }
}

public class BlockNode : ASTNode, IReturnAny
{
    public BlockNode() => Executable = false;
    public List<Name> LocalNames { get; } = new();

    public object AnyValue => Nodes.OfType<IReturnAny>().Select(a => a.AnyValue).ToArray();

    public void FreeLocalNames()
    {
        LocalNames.ForEach(Ugh.FreeName);
        LocalNames.Clear();
    }
}


public class ExpressionNode : ASTNode, IReturnAny
{
    public object AnyValue => Express();
    private object Express() => Operation.Express(Nodes);
}
public class ArrayNode : AssignedNode<IReturnAny>, IReturn<object[]>
{
    public int Index
    {
        get
        {
            try { return (int)Assigned.AnyValue; }
            catch(Exception ex) { Debug.Error(ex.Message); }
            return 0;
        }
    }
        
    public object AnyValue => new object[Index];
    
    public object[] Value { get; set; } = Array.Empty<object>();
}


/// <summary>
/// Used to declare and set variables
/// </summary>

public class NameNode : ASTNode, IReturnAny 
{
    public required Token Token { get; init; }

    public object AnyValue => GetValue();
    
    private IReturnAny? any;
    private OperatorNode? oprNode;
    private ArrayNode? arrayNode;
    
    private BaseFunction? fun;
    private IEnumerable<IReturnAny> args = [];
    
    public override void Load()
    {
        base.Load();

        if (Parent.CheckType<INamed>()) return;
        
        if (TryGetNode<OperatorNode>(0, out oprNode))
            any =  HandleGetNode<IReturnAny>(1);
        else if (TryGetNode<ExpressionNode>(0, out var exprsNode) && Ugh.TryGetName(Token.StringValue, out Name n)) 
        {
            fun = n.GetAs<BaseFunction>();
            args = exprsNode.GetNodes<IReturnAny>();
        }
        else if (TryGetNode<ArrayNode>(0, out arrayNode) && TryGetNode<OperatorNode>(1, out oprNode))
            any = HandleGetNode<IReturnAny>(2);
    }

    public override void Execute()
    {
        base.Execute();
        
        if (oprNode is null || any is null)
        {
            fun?.Invoke(args);
            return;
        }

        if (Ugh.TryGetName(Token.StringValue, out var variable))
        {
            if (arrayNode is not null)
            {
                var array = variable.Value as object[];
                array![arrayNode.Index] = Operation.Operate(array[arrayNode.Index], any.AnyValue, oprNode.Operator);
            }
            else
                variable.Value = Operation.Operate(variable.Value, any.AnyValue, oprNode.Operator);
        }
        else // Initialization
        {
            var v = Parent is ConstNode? new Constant(Token.StringValue, any.AnyValue) : new Variable(Token.StringValue, any.AnyValue);
            Ugh.RegisterName(v);

            if (Parent is BlockNode blockNode) // deregister variable after end of tag node execution
                blockNode.LocalNames.Add(v);
        }
    }
    
    public Name GetName() => Ugh.GetName(Token.StringValue); // TODO: Make this variable
    public object GetValue()
    {
        var name = GetName();
        if (arrayNode is null) return name.AnyValue;
        
        var array = name.AnyValue as object[];
        return array![arrayNode.Index];
    }
}

public class OperableNameNode : NameNode, IOperable;
