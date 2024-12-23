using UghLang.Modules;
namespace UghLang.Nodes;

/// <summary>
/// Writes value 
/// </summary>
public class PrintNode : AssignedNode<IReturnAny>
{
    public override void Execute()
    {
        base.Execute();
        Console.WriteLine(assigned?.AnyValue);
    }
}

/// <summary>
/// Creates input field and writes value 
/// </summary>
public class InputNode : AssignedNode<IReturnAny>, IReturn<string>, IOperatable
{
    public string Value { get; set; } = string.Empty;
    public object AnyValue => Value;

    public override void Execute()
    {
        base.Execute();
        Console.Write(assigned?.AnyValue);
        Value = Console.ReadLine() ?? string.Empty;
    }
}

/// <summary>
/// Disposes Name from Ugh
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
public class DeclareFunctionNode : ASTNode, INamed
{
    private NameNode? name;

    public override void Load()
    {
        base.Load();

        name = GetNode<NameNode>(0);


        Function fun = new(name.Token.StringValue, GetNode<TagNode>(1), name.GetNode<ExpressionNode>(0));
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
/// Sets current executed function value and breaks it whole execution
/// </summary>
public class ReturnNode : AssignedNode<IReturnAny>, IReturnAny
{
    public Function Function => Ugh.Function as Function ?? throw new InvalidSpellingException(this);
    public object AnyValue => Function.AnyValue;

    public override void Execute()
    {
        base.Execute();
            
        if(assigned is not null) 
            Function.Value = assigned.AnyValue;

        Function.TagNode.Executable = false;
    }
}

public class IfNode : AssignedIReturnAnyAndTagNode
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


        if ((bool)Assigned.AnyValue == true)
            Tag?.ForceExecute();
        else if (elseNode is not null) elseNode.Executable = true;
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
public class RepeatNode : AssignedIReturnAnyAndTagNode
{
    public override void Execute()
    {
        base.Execute();

        Tag.Executable = true;
        for (int i = 0; i < (int)Assigned.AnyValue; i++)
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
public class WhileNode : AssignedIReturnAnyAndTagNode
{
    public override void Execute()
    {
        base.Execute();

        Tag.Executable = (bool)Assigned.AnyValue;
        while ((bool)Assigned.AnyValue)
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

        if(File.Exists(path)) { }
        else if (Path.Exists(path)) { path += "/source.ugh"; }
        else path = Path.GetDirectoryName(Environment.ProcessPath) ?? "" + $"/{path}/source.ugh";

        var file = File.ReadAllText(path);

        var parser = new Parser(Ugh, true);
        var lexer = new Lexer(file, parser);
        
        parser.LoadAndExecute();
    }
}

/// <summary>
/// Marks node as local. Local nodes can't be inserted to other file
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

public class ModuleNode : ASTNode
{
    public override void Load()
    {
        base.Load();
        var strNode = GetNode<ConstStringValueNode>(0);
        var asNode = GetNodeOrDefalut<AsNode>(1);

        var name = asNode is null? strNode.Value : asNode.Assigned.Value;

        foreach (var method in ModuleLoader.LoadModuleMethods(strNode.Value))
        {
            ModuleFunction function = new($"{name}.{method.Key}",Ugh, method.Value);
            Ugh.RegisterName(function);
        }
    }
}

public class AsNode : AssignedNode<ConstStringValueNode>;