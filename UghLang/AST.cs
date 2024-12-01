namespace UghLang;


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
    public Token GetToken(int index)
    {
        if (index >= tokens.Count)
            return Token.NULL;
        return tokens[index];
    }

    public void AddValue(Token token) => tokens.Add(token);



    public abstract void Execute();
}

public class RefrenceNode : ASTNode
{
    public override void Execute()
    {

    }
}

public class PrintNode : ASTNode
{
    public override void Execute()
    {
        Console.WriteLine(GetToken(0).GetValue().Get(Parent.master));
    }
}

public class VarNode : ASTNode
{
    public override void Execute()
    {
        string name = GetToken(0).Value;
        Token val = GetToken(1);
        Parent.master.DeclareVariable(name, val.Value, val.GetDataType());
    }
}
public class FreeNode : ASTNode
{
    public override void Execute()
    {
        Token token = GetToken(0);
        if(token == Token.NULL)
        {
            Parent.master.FreeAllVariables();
            return;
        }
        string name = GetToken(0).Value;
        Parent.master.FreeVariable(name);
    }
}