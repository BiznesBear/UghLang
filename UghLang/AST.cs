namespace UghLang;

// TODO: Rework this couse it useless
public class AST(Master m)
{
    public readonly Master master = m;
    private List<ASTNode> nodes = new();
    public void AddNode(ASTNode node)
    {
        node.Parent = this;
        nodes.Add(node);
    }

    public void Execute()
    {
        foreach (ASTNode node in nodes)
            node.Execute();
    }
}



public abstract class ASTNode
{
    protected readonly List<Token> tokens = new();

    private AST? parent;
    public AST Parent
    {
        get => parent ?? throw new NullReferenceException("No assigned parent in node");
        set => parent = value;
    }
    public Master Master => Parent.master;


    public bool TryGetToken(int index, out Token token)
    {
        if (index >= tokens.Count)
        {
            token = Token.NULL;
            return false;
        }
        token = tokens[index];
        return true;
    }
    public Token GetToken(int index)
    {
        if (index >= tokens.Count)
            return Token.NULL;
        return tokens[index];
    }

    public void AddToken(Token token) => tokens.Add(token);
    public abstract void Execute();
}


public class NullNode : ASTNode { public override void Execute() { } }

public class PrintNode : ASTNode
{
    public override void Execute()
    {
        if(TryGetToken(0, out Token t))
        {
            object val = t.Value;
            if(Master.TryGetVariable(t.StringValue, out Variable? var))
            {
                val = var.Get();
            }
            Console.WriteLine(val);
        }
    }
}

public class VarNode : ASTNode
{
    public override void Execute()
    {
        string name = GetToken(0).StringValue;
        Token baseValueToken = GetToken(1);
        Master.DeclareVariable(name, baseValueToken);
    }
}
public class FreeNode : ASTNode
{
    public override void Execute()
    {
        if(!TryGetToken(0,out Token t))
        {
            Master.FreeAllVariables();
            return;
        }
        string name = t.StringValue;
        Master.FreeVariable(name);
    }
}

public class RefrenceNode : ASTNode
{
    public override void Execute()
    {
        var variable = Master.GetVariable(GetToken(0).StringValue);

        var oprToken = GetToken(1);

        var valToken = GetToken(2);
        
        dynamic rVal = valToken.Value;
        if(Master.TryGetVariable(valToken.StringValue, out var val)) rVal = val;

        Operation operation = new(variable.Get(), rVal, oprToken.Operator ?? default);

        variable.Set(operation.GetResult());
    }
}