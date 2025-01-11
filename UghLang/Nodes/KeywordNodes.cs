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
public class InputNode : AssignedNode<IReturnAny>, IReturn<string>, IOperable
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
    private InitializeNode? refNode;
    public override void Load()
    {
        base.Load();
        refNode = GetNodeOrDefalut<InitializeNode>(0);
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
    private InitializeNode? name;

    public override void Load()
    {
        base.Load();
        name = HandleGetNode<InitializeNode>(0);

        Function fun = new(name.Token.StringValue, HandleGetNode<BlockNode>(1), name.HandleGetNode<ExpressionNode>(0));
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
public class ReturnNode : AssignedNode<IReturnAny>
{
    private Function Function => Ugh.Function ?? throw new InvalidSpellingException(this);

    public override void Execute()
    {
        base.Execute();
        
        if(assigned is not null) 
            Function.Value = assigned.AnyValue;
        Function.BlockNode.Executable = false;
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

        
        if ((bool)Assigned.AnyValue)
            Block?.ForceExecute();
        else if (elseNode is not null) elseNode.Executable = true;
        
        Block?.FreeLocalNames();
    }
}

public class ElseNode : AssignedNode<BlockNode>
{
    public ElseNode() => Executable = false;

    public override void Execute()
    {
        if (!Executable) return;

        base.Execute();
        Assigned.ForceExecute();
        Assigned?.FreeLocalNames();
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

        Block.Executable = true;
        for (int i = 0; i < (int)Assigned.AnyValue; i++)
        {
            if (!Block.Executable) break;
            Block.Execute();
        }
        Block.Executable = false;
        Block.FreeLocalNames();
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

        Block.Executable = (bool)Assigned.AnyValue;
        while ((bool)Assigned.AnyValue)
        {
            if (!Block.Executable) break;
            Block.Execute();
        }
        Block.Executable = false;
        Block.FreeLocalNames();
    }
}

public class ForeachNode : ASTNode
{
    private InitializeNode? itemNode;
    private InitializeNode? collectionNode;
    private BlockNode? blockNode;
    
    public override void Load()
    {
        base.Load();
        itemNode = HandleGetNode<InitializeNode>(0);
        collectionNode = HandleGetNode<InitializeNode>(1);
        blockNode = HandleGetNode<BlockNode>(2);
    }

   
    public override void Execute()
    {
        base.Execute();
        if(itemNode is null || collectionNode is null || blockNode is null) 
            throw new InvalidSpellingException(this); 

        
        var collection = collectionNode.AnyValue as object[] ?? throw new InvalidSpellingException(this);
        
        Variable item = new(itemNode.Token.StringValue, 0);
        
        Ugh.RegisterName(item);

        blockNode.Executable = true;
        foreach (var obj in collection)
        {
            item.Value = obj;
            blockNode.Execute();
        }
        
        blockNode.Executable = false;
        blockNode.FreeLocalNames();
        Ugh.FreeName(item);
    }
}

/// <summary>
/// Imports other ugh files and mark them as inserted
/// </summary>
public class InsertNode : ASTNode 
{
    public override void Load()
    {
        base.Load();
        
        var path = HandleGetNode<ConstStringValueNode>(0).Value;

        if(File.Exists(path)) { }
        else if (Path.Exists(path)) { path += "/source.ugh"; }

        var file = File.ReadAllLines(path);

        using var parser = new Parser(Ugh, true);
        using var lexer = new Lexer(file, parser);
        
        parser.LoadAndExecute();
    }
}

/// <summary>
/// Marks children as local which makes them invisible for inserter. 
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
    
        if (TryGetNode<BlockNode>(0, out var tag)) // Nested local
            tag.Executable = true;
    
        base.Load();
    }
}

public class ModuleNode : ASTNode
{
    public override void Load()
    {
        base.Load();
        var strNode = HandleGetNode<ConstStringValueNode>(0);
        var asNode = GetNodeOrDefalut<AsNode>(1);
        
        var name = asNode is null? strNode.Value : asNode.Assigned.Value;

        foreach (var method in ModuleLoader.LoadModuleMethods(strNode.Value))
        {
            ModuleFunction function = new($"{name}.{method.Key}", Ugh, method.Value);
            Ugh.RegisterName(function);
        }
    }
}

public class AsNode : AssignedNode<ConstStringValueNode>;