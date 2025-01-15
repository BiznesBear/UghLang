using UghLang.Modules;
namespace UghLang.Nodes;


public interface IKeywordNode;

/// <summary>
/// Writes value 
/// </summary>
public class PrintNode : AssignedNode<IReturnAny>, IKeywordNode
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
public class InputNode : AssignedNode<IReturnAny>, IReturn<string>, IOperable, IKeywordNode
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
public class FreeNode : ASTNode, IKeywordNode
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
public class DeclareFunctionNode : ASTNode, INamed, IKeywordNode
{
    private NameNode? name;

    public override void Load()
    {
        base.Load();
        name = HandleGetNode<NameNode>(0);

        Function fun = new(name.Token.StringValue, HandleGetNode<BlockNode>(2), HandleGetNode<ExpressionNode>(1));
        Ugh.RegisterName(fun);
    }
}

/// <summary>
/// Breaks parent execution
/// </summary>
public class BreakNode : ASTNode, IKeywordNode
{
    public override void Execute() => Parent.Executable = false;
}

/// <summary>
/// Sets current executed function value and breaks it whole execution
/// </summary>
public class ReturnNode : AssignedNode<IReturnAny>, IKeywordNode
{
    public override void Execute()
    {
        base.Execute();

        if (Ugh.Function is not null)
        {
            if (assigned is not null)
                Ugh.Function.Value = assigned.AnyValue;
            Ugh.Function.BlockNode.Executable = false;
        }
        else if (Ugh.ReturnBlock is not null)
            Ugh.ReturnBlock.Executable = false;
        else
            Parent.Executable = false;
    }
}

public class IfNode : AssignedIReturnAnyAndBlockNode, IKeywordNode
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

public class ElseNode : AssignedNode<BlockNode>, IKeywordNode
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

public class ElifNode : IfNode, IKeywordNode
{
    public ElifNode() => Executable = false;
}

/// <summary>
/// Creates for loop from 0 to (value)
/// </summary>
public class RepeatNode() : ASTNode, INamed, IKeywordNode
{
    private IReturnAny? minNode;
    private IReturnAny? maxNode;
    private NameNode? nameNode;
    private BlockNode? blockNode;

    private BlockNode BlockNode => blockNode ?? throw new InvalidSpellingException(this);

    public override void Load()
    {
        base.Load();
        minNode = HandleGetNode<IReturnAny>(0);
        maxNode = HandleGetNode<IReturnAny>(1);
        nameNode = GetNodeOrDefalut<NameNode>(2);
        blockNode = HandleGetNode<BlockNode>(nameNode is null? 2 : 3);
    }

    public override void Execute()
    {
        base.Execute();

        
        int min = (int)(minNode?.AnyValue ?? 0);
        int max = (int)(maxNode?.AnyValue ?? 0);

        Variable? var = nameNode is null? null : new(nameNode?.Token.StringValue ?? string.Empty, min);

        if(var is not null)
        {
            Ugh.RegisterName(var);
            BlockNode.LocalNames.Add(var);
        }

        Ugh.SetReturnBlock(BlockNode);

        BlockNode.Executable = true;

        if (min < max)
            for (int i = min; i < max; i++)
            {
                if (!BlockNode.Executable) break;
                Do(i);
            }
        else
            for (int i = min - 1; i >= max; i--)
            {
                if (!BlockNode.Executable) break;
                Do(i);
            }

        BlockNode.Executable = false;
        BlockNode.FreeLocalNames();
        Ugh.SetReturnBlock(null);

        void Do(int i)
        {
            if (var is not null) var.Value = i;
            BlockNode.Execute();
        }
    }
}

/// <summary>
/// Creates new while loop
/// </summary>
public class WhileNode : AssignedIReturnAnyAndBlockNode, IKeywordNode
{
    public override void Execute()
    {
        base.Execute();

        Ugh.SetReturnBlock(Block);
        Block.Executable = (bool)Assigned.AnyValue;
        while ((bool)Assigned.AnyValue)
        {
            if (!Block.Executable) break;
            Block.Execute();
        }
        Block.Executable = false;
        Block.FreeLocalNames();
        Ugh.SetReturnBlock(null);
    }
}

public class ForeachNode : ASTNode, IKeywordNode
{
    private NameNode? itemNode;
    private NameNode? collectionNode;
    private BlockNode? blockNode;
    
    public override void Load()
    {
        base.Load();
        itemNode = HandleGetNode<NameNode>(0); // idk why this node can work without INamed but who cares
        collectionNode = HandleGetNode<NameNode>(1);
        blockNode = HandleGetNode<BlockNode>(2);
    }

   
    public override void Execute()
    {
        base.Execute();
        if(itemNode is null || collectionNode is null || blockNode is null) 
            throw new InvalidSpellingException(this);
        
        Ugh.SetReturnBlock(blockNode);

        var collection = collectionNode.AnyValue as IList<object> ?? throw new InvalidSpellingException(this);
        
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
        Ugh.SetReturnBlock(null);
    }
}

/// <summary>
/// Imports other ugh files and mark them as inserted
/// </summary>
public class InsertNode : ASTNode, IKeywordNode
{
    public override void Load()
    {
        base.Load();
        
        var path = HandleGetNode<ConstStringValueNode>(0).Value;

        if(File.Exists(path)) { }
        else if (Path.Exists(path)) { path += "/master.ugh"; }

        var file = File.ReadAllText(path);

        using var parser = new Parser(Ugh, true);
        using var lexer = new Lexer(file, parser);
        
        parser.LoadAndExecute();
    }
}

/// <summary>
/// Marks children as local which makes them invisible for inserter. 
/// </summary>
public class LocalNode : ASTNode, ITag, IKeywordNode
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

public class ModuleNode : ASTNode, INamed, IKeywordNode
{
    public override void Load()
    {
        base.Load();

        var strNode = HandleGetNode<ConstStringValueNode>(0);
        var asNode = GetNodeOrDefalut<AsNode>(1);

        var fromNode = asNode?.GetNodeOrDefalut<FromNode>(1);
        fromNode ??= GetNodeOrDefalut<FromNode>(1);
        
        asNode ??= fromNode?.GetNodeOrDefalut<AsNode>(1);

        var name = asNode is null? strNode.Value : asNode.GetStringName();
        var module = ModuleLoader.LoadModule(strNode.Value, fromNode?.Assigned.Value);

        foreach (var method in module.Methods)
        {
            ModuleFunction function = new($"{name}.{method.Key}", Ugh, method.Value);
            Ugh.RegisterName(function);
        }

        foreach (var field in module.Fields)
        {
            Constant con = new($"{name}.{field.Key}", field.Value.GetValue(null) ?? 0);
            Ugh.RegisterName(con);
        }
    }
}

public class AsNode : AssignedNode<NameNode>, INamed, IKeywordNode
{
    public string GetStringName() => Assigned.Token.StringValue;
}
public class FromNode : AssignedNode<ConstStringValueNode>, IKeywordNode;

public class ConstNode : ASTNode, ITag, IKeywordNode;