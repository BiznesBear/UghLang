namespace UghLang.Nodes;
#region IO
public class PrintNode : ASTNode
{
    public override void Execute()
    {
        base.Execute();
        if (!TryGetNodeWith<ExpressionNode>(out var expr))
            throw new InvalidSpellingException(this);
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
            throw new InvalidSpellingException(this);
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
        else throw new InvalidSpellingException(this);
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
           throw new InvalidSpellingException(this);


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
    private ASTNode? elseNode; // TODO: Add implementation for else node

    public override void Load()
    {
        base.Load();

        exprs = GetNodeWith<ExpressionNode>();
        tag = GetNodeWith<TagNode>();
        elseNode = GetNextBrother<ElseNode>();
        elseNode ??= GetNextBrother<ElifNode>(); // if else node is null
    }

    public override void Execute()
    {
        base.Execute();
        if (exprs is not null)
        {
            if ((bool)exprs.AnyValue == true)
                tag?.BaseExecute();
            else if (elseNode is not null) elseNode.CanExecute = true;
        }
        else throw new InvalidSpellingException(this);
    }
}

public class ElseNode : ASTNode
{
    public ElseNode() => CanExecute = false;

    private TagNode? tag;

    public override void Load()
    {
        base.Load();
        if (TryGetNodeWith<TagNode>(out var tn))
            tag = tn;
        else throw new InvalidSpellingException(this);
    }

    public override void Execute()
    {
        if (!CanExecute) return;
        base.Execute();
        tag?.BaseExecute();
    }
}
public class ElifNode : IfNode
{
    public ElifNode() => CanExecute = false;
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
        
        if (!TryGetNodeWith<StringValueNode>(out var expr)) throw new InvalidSpellingException(this);
        string path = expr.Value;

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
public class ReturnNode : ASTNode
{
    public override void Execute()
    {
        base.Execute();
        throw new NotImplementedException("Return is not compleate function. Please remove it from your code.");
    }
}