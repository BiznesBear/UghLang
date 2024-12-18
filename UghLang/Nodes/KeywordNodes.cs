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

        if (exprs is null || tag is null || name is null) // TODO: Remove this from everywere
           throw new InvalidSpellingException(this);
        
        Function fun = new(name.Token.StringValue, tag, exprs);
        Ugh.RegisterName(fun);
    }
}

/// <summary>
/// Breaks parent execution
/// </summary>
public class BreakNode : ASTNode
{
    public override void Execute() => Parent.Executable = false;
}
public class ReturnNode : NestedExpressionNode 
{
    public override void Execute()
    {
        base.Execute();

        // break master branch (not the parent like in break keyword)
        // set value from exprs to current executed function

        if(Ugh.Function is null || exprs is null)
            throw new InvalidSpellingException(this);

        Ugh.Function.Value = exprs.AnyValue;
        Ugh.Function.TagNode.Executable = false;
    }
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
        if (!Executable) return;
        base.Execute();

        if (exprs is null) throw new InvalidSpellingException(this);

        if ((bool)exprs.AnyValue == true)
            tag?.ForceExecute();
        else if(elseNode is not null) elseNode.Executable = true;
    }
}

public class ElseNode : ASTNode
{
    public ElseNode() => Executable = false;

    private TagNode? tag;

    public override void Load()
    {
        base.Load();
        tag = GetNode<TagNode>(0);
    }

    public override void Execute()
    {
        if (!Executable) return;

        base.Execute();
        tag?.ForceExecute();
    }
}

public class ElifNode : IfNode
{
    public ElifNode() => Executable = false;
}

public class ForNode : NestedExpressionAndTagNode
{
    public override void Execute()
    {
        base.Execute();

        if (exprs is null || tag is null) throw new InvalidSpellingException(this);
        
        tag.Executable = true;

        for (int i = 0; i < (int)exprs.AnyValue; i++)
        {
            if (!tag.Executable) break;
            tag.Execute();
        }
        tag.Executable = false;

    }
}

public class WhileNode : NestedExpressionAndTagNode
{
    public override void Execute()
    {
        base.Execute();
        if (exprs is null || tag is null) throw new InvalidSpellingException(this);

        tag.Executable = (bool)exprs.AnyValue;

        while ((bool)exprs.AnyValue)
        {
            if (!tag.Executable) break;
            tag.Execute();
        }
        tag.Executable = false;
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

        var parser = new Parser(Ugh, true);
        var lexer = new Lexer(file, parser);
        
        parser.Execute();
    }
}



public class LocalNode : ASTNode
{
    public override void Load()
    {
        if (Parser.Inserted)
        {
            Executable = false;
            return;
        }

        if (TryGetNode<TagNode>(0, out var tag)) // Nested local
            tag.Executable = true;

        base.Load();
    }
}