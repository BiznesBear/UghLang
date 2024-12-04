namespace UghLang;


public abstract class ASTNode
{
    protected readonly List<Token> tokens = new();
    protected List<ASTNode> Children { get; } = new();

    private Master? master;
    public Master Master
    {
        get => master ?? throw new NullReferenceException("No assigned AST in node");
        set => master = value;
    }

    private ASTNode? parent;
    public ASTNode Parent
    {
        get => parent ?? throw new NullReferenceException("No assigned parent in node");
        set => parent = value;
    }


    public void AddNode(ASTNode node)
    {
        node.Parent = this;
        node.Master = Master;
        Children.Add(node);
    }
    public void AddToken(Token token) => tokens.Add(token);

    public bool TryGetToken(int index, out Token token)
    {
        if (index >= tokens.Count)
        {
            token = Token.NULL_STR;
            return false;
        }
        token = tokens[index];
        return true;
    }
    public Token GetToken(int index)
    {
        if (index >= tokens.Count)
            return Token.NULL_STR;
        return tokens[index];
    }

    public abstract void Execute();
}


public class AST : ASTNode
{
    public AST(Master m) => Master = m;
    public override void Execute()
    {
        foreach (ASTNode node in Children)
            node.Execute();
    }
}

public class PrintNode : ASTNode
{
    public override void Execute()
    {
        string sentence = string.Empty;
        foreach(Token t in tokens)
        {
            object val = t.Value;
            if (Master.TryGetVariable(t, out Variable? var))
                val = var.Get();
            sentence += val;
        }
        Console.WriteLine(sentence);
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
        Master.FreeVariable(t);
    }
}

public class RefrenceNode : ASTNode
{
    public override void Execute()
    {
        var token = GetToken(0);
        var oprToken = GetToken(1);
        var valToken = GetToken(2);

        // check if right value is variable
        dynamic rVal = valToken.Value;
        if (Master.TryGetVariable(valToken, out var val)) rVal = val;


        // create new variable if 
        Variable variable;
        if (Master.TryGetVariable(token, out Variable var)) variable = var;
        else
        {
            variable = new(token, valToken);
            Master.DeclareVariable(variable);
        }


        Operation operation = new(variable.Get(), rVal, oprToken.Operator ?? default);
        variable.Set(operation.GetResult());
    }
}