namespace UghLang.Nodes;

public interface IExpressable { }

/// <summary>
/// Writes line with expression in console
/// </summary>
public class PrintNode : NestedExpressionNode
{
    public override void Execute()
    {
        base.Execute();
        Console.WriteLine(Expression.AnyValue);
    }
}

/// <summary>
/// Creates input field and writes expression in console
/// </summary>
public class InputNode : NestedExpressionNode, IReturn<string>
{
    public string Value { get; set; } = string.Empty;
    public object AnyValue => Value;

    public override void Execute()
    {
        base.Execute();
        Console.Write(Expression.AnyValue);
        Value = Console.ReadLine() ?? string.Empty;
    }
}

/// <summary>
/// Releases name from ugh
/// </summary>
public class FreeNode : ASTNode
{
    private NameNode? refNode;

    public override void Load()
    {
        base.Load();
        
        refNode = GetNodeOrDefalut<NameNode>(0);
    }

    public override void Execute()
    {
        base.Execute();
        if (refNode is not null)
            Ugh.FreeName(refNode.GetName());
        else if (TryGetNode<ConstStringValueNode>(0,out var modifier) && modifier.Value == "all")
            Ugh.FreeAll();
        else throw new InvalidSpellingException(this);
    }

}

/// <summary>
/// Declares new function 
/// </summary>
public class DeclareFunctionNode : NestedExpressionAndTagNode
{
    public DeclareFunctionNode() : base(1, 2) { }

    private NameNode? name;

    public override void Load()
    {
        base.Load();

        name = GetNode<NameNode>(0);

        if (name is null) 
           throw new InvalidSpellingException(this);
        
        Function fun = new(name.Token.StringValue, Tag, Expression);
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

/// <summary>
/// Sets current executed function value and breaks it execution
/// </summary>
public class ReturnNode : NestedExpressionNode, IReturnAny
{
    public Function Function => Ugh.Function ?? throw new InvalidSpellingException(this);
    public object AnyValue => Function.AnyValue;

    public override void Execute()
    {
        base.Execute();
            
        if(exprs is not null) 
            Function.Value = exprs.AnyValue;

        Function.TagNode.Executable = false;
    }
}

public class CallNode() : NestedExpressionNode(1), IReturnAny, IExpressable
{
    public object AnyValue => Function.AnyValue;
    public Function Function => fun ?? throw new InvalidSpellingException(this);

    private Function? fun;
    private IEnumerable<IReturnAny> args = [];

    public override void Load()
    {
        base.Load();
        fun = GetNode<NameNode>(0).GetName().Get<Function>();
        args = Expression.GetNodes<IReturnAny>();
    }

    public override void Execute()
    {
        base.Execute();
        Function.Invoke(args);
    }
}

/// <summary>
/// Checks if expression equals true
/// </summary>
public class IfNode : NestedExpressionAndTagNode
{
    private ASTNode? elseNode; 

    public override void Load()
    {
        base.Load();

        elseNode = GetNextBrother<ElseNode>();
        elseNode ??= GetNextBrother<ElifNode>(); // check of elif node
    }

    public override void Execute()
    {
        if (!Executable) return;
        base.Execute();


        if ((bool)Expression.AnyValue == true)
            Tag?.ForceExecute();
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

/// <summary>
/// Creates for loop from 0 to (value)
/// </summary>
public class ForNode : NestedExpressionAndTagNode
{
    public override void Execute()
    {
        base.Execute();

        Tag.Executable = true;
        for (int i = 0; i < (int)Expression.AnyValue; i++)
        {
            if (!Tag.Executable) break;
            Tag.Execute();
        }
        Tag.Executable = false;

    }
}

/// <summary>
/// Creates new while loop
/// </summary>
public class WhileNode : NestedExpressionAndTagNode
{
    public override void Execute()
    {
        base.Execute();

        Tag.Executable = (bool)Expression.AnyValue;
        while ((bool)Expression.AnyValue)
        {
            if (!Tag.Executable) break;
            Tag.Execute();
        }
        Tag.Executable = false;
    }
}

/// <summary>
/// Imports other ugh files and mark them as Inserted
/// </summary>
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
        
        parser.LoadAndExecute();
    }
}

/// <summary>
/// Marks node as local. Local nodes will not be imported
/// </summary>
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

