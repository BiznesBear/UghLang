namespace UghLang.Nodes;
#region IO
public class PrintNode : ASTNode
{
    public override void Execute()
    {
        base.Execute();
        if (!TryGetNodeWith<ExpressionNode>(out var expr)) 
            throw new UghException("Invalid spelling of print keyword");
        Console.WriteLine(expr.AnyValue);

    }
}
public class InputNode : AnyValueNode<string>
{
    public InputNode() : base(string.Empty) { }

    public override void Execute()
    {
        base.Execute();
        if (!TryGetNodeWith<ExpressionNode>(out var expr))
            throw new UghException("Invalid spelling of input keyword");
        Console.Write(expr.AnyValue);
        Value = Console.ReadLine() ?? string.Empty;
    }
}
#endregion

public class FreeNode : ASTNode
{
    private bool isRef;
    private NameNode? refNode;

    public override void Load()
    {
        base.Load();
        isRef = TryGetNodeWith(out NameNode refr);
        refNode = refr;
    }

    public override void Execute()
    {
        base.Execute();
        if (isRef && refNode is not null)
            Ugh.FreeName(refNode.GetName());
        else if (TryGetNodeWith<AnyValueNode<string>>(out var modifier) && modifier.Value == "all")
            Ugh.FreeAll();
        else throw new UghException("Invalid initialization of free keyword");
    }

}
public class DeclareFunctionNode : ASTNode
{
    private ExpressionNode? exprs;
    private TagNode? tag;
    private NameNode? name;


    public override void Load()
    {
        base.Load();

        exprs = GetNodeWith<ExpressionNode>();
        tag = GetNodeWith<TagNode>();
        name = GetNodeWith<NameNode>();

        if (exprs is null || tag is null || name is null) 
            throw new UghException("Wrong implementation of fun");


        Function fun = new(name.Token.StringValue, tag, exprs);
        Ugh.RegisterName(fun);
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


public class IfNode : ASTNode
{
    private ExpressionNode? exprs;
    private TagNode? tag;
    private ElseNode? elseNode; // TODO: Add implementation for else node

    public override void Load()
    {
        base.Load();

        exprs = GetNodeWith<ExpressionNode>();
        tag = GetNodeWith<TagNode>();
        elseNode = GetNextBrother<ElseNode>();
    }

    public override void Execute()
    {
        base.Execute();
        if (exprs is not null)
        {
            if ((bool)exprs.AnyValue == true)
            {
                tag?.BaseExecute();
                if (elseNode is not null) elseNode.CanExecute = false;
            }
        }
        else throw new UghException("Cannot find expression in if statement");
    }
}


public class ElseNode : ASTNode
{
    private TagNode? tag;
    public override void Load()
    {
        base.Load();
        tag = GetNodeWith<TagNode>();
    }
    public override void Execute()
    {
        if (!CanExecute) return;
        base.Execute();
        tag?.BaseExecute();
    }
}


public class RepeatNode : ASTNode
{
    private ExpressionNode? exprs;
    private TagNode? tag;

    public override void Load()
    {
        base.Load();
        exprs = GetNodeWith<ExpressionNode>();
        tag = GetNodeWith<TagNode>();
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
public class InsertNode : ASTNode
{
    public override void Load()
    {
        base.Load();
        if (!TryGetNodeWith<ExpressionNode>(out var expr)) throw new UghException("Invalid spelling of insert keyword");
        string path = (string)expr.AnyValue;

        if (File.Exists(path)) { }
        else if (Path.Exists(path))
            path += "/source.ugh";
        else throw new UghException("Cannot find ugh named " + path);

        var file = File.ReadAllText(path);

        var parser = new Parser(Ugh);
        var lexer = new Lexer(file, parser);
        parser.Execute();
    }
}