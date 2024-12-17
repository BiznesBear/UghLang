namespace UghLang.Nodes;


public class PrintNode : NestedExpressionNode
{
    public override void Execute()
    {
        base.Execute();
        Console.WriteLine(exprs?.AnyValue);
    }
}

public class InputNode : NestedExpressionNode, IReturn<string>
{
    public string Value { get; set; } = string.Empty;
    public object AnyValue => Value;

    public override void Execute()
    {
        base.Execute();
        Console.Write(exprs?.AnyValue);
        Value = Console.ReadLine() ?? string.Empty;
    }
}


public class FreeNode : ASTNode
{
    private bool isRef;
    private NameNode? refNode;

    public override void Load()
    {
        base.Load();
        isRef = TryGetNode(0, out NameNode refr);
        refNode = refr;
    }

    public override void Execute()
    {
        base.Execute();
        if (isRef && refNode is not null)
            Ugh.FreeName(refNode.GetName());
        else if (TryGetNode<ConstStringValueNode>(0,out var modifier) && modifier.Value == "all")
            Ugh.FreeAll();
        else throw new InvalidSpellingException(this);
    }

}

public class DeclareFunctionNode : NestedExpressionAndTagNode
{
    public DeclareFunctionNode() : base(1, 2) { }

    private NameNode? name;

    public override void Load()
    {
        base.Load();
        name = GetNode<NameNode>(0);

        if (exprs is null || tag is null || name is null) 
           throw new InvalidSpellingException(this);


        Function fun = new(name.Token.StringValue, tag, exprs);
        Ugh.RegisterName(fun);
    }
}


public class BreakNode : ASTNode
{
    public override void Execute() => Parent.CanExecute = false;
}


public class IfNode : NestedExpressionAndTagNode
{
    private ASTNode? elseNode; // TODO: Add implementation for else node

    public override void Load()
    {
        base.Load();

        elseNode = GetNextBrother<ElseNode>();
        elseNode ??= GetNextBrother<ElifNode>(); // if 'else' node is null
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
        tag = GetNode<TagNode>(0);
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

public class ForNode : NestedExpressionAndTagNode
{
    public override void Execute()
    {
        base.Execute();
        if (exprs is null) throw new InvalidSpellingException(this);

        for (int i = 0; i < (int)exprs.AnyValue; i++) tag?.BaseExecute();
    }
}

public class WhileNode : NestedExpressionAndTagNode
{
    public override void Execute()
    {
        base.Execute();
        if (exprs is null) throw new InvalidSpellingException(this);

        while ((bool)exprs.AnyValue)
            tag?.BaseExecute();
    }
}

public class InsertNode : ASTNode 
{
    public override void Load()
    {
        base.Load();
        
        string path = GetNode<ConstStringValueNode>(0).Value;

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
        throw new NotImplementedException("Please 'return' to README.md and read it");
    }
}