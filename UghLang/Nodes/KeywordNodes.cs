using System.Reflection;
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
public class FreeNode : AssignedNode<NameNode>, IKeywordNode
{
    public override void Execute()
    {
        base.Execute();
        if (assigned is not null)
            Ugh.FreeName(assigned.Name);
        else if (TryGetNode<ConstNullValueNode>(0, out _))
            Ugh.FreeAll();
        else throw new InvalidSpellingException(this);
    }
}

/// <summary>
/// Declares new function 
/// </summary>
public class DeclareFunctionNode : ASTNode, INaming, IKeywordNode
{
    private Function? function;

    public override void Load()
    {
        base.Load();
        var name = GetNode<NameNode>(0).Token.StringValue;
        var blockNode = GetNode<BlockNode>(2);
        var localVars = GetNode<ExpressionNode>(1).GetNodes<NameNode>().Select(nn => nn.Token.StringValue).ToArray();

        function = new(name, blockNode, localVars);
    }

    public override void Execute()
    {
        base.Execute();
        Ugh.RegisterName(function!);
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
        else if (Ugh.ReturnStack.TryPeek(out var peek)) // Prevents from breaking ifs, elifs etc.
            peek.Executable = false;
        else
            Parser.AST.Executable = false;
    }
}

public class IfNode : AssignedIReturnAnyAndBlockNode, IKeywordNode
{
    private ASTNode? elseNode;
    public void SetElseNode(ASTNode node) => elseNode = node;

    public override void Execute()
    {
        base.Execute();

        if (Assigned.GetAny<bool>())
            Block.ForceExecute();
        else if (elseNode is not null) elseNode.Executable = true;
        Block.FreeLocalNames();
    }
}

public class ElseNode : AssignedNode<BlockNode>, IKeywordNode
{
    public ElseNode() => Executable = false;

    public override void Load()
    {
        base.Load();
        ParseIf(this);
    }

    public override void Execute()
    {
        base.Execute();
        Assigned.ForceExecute();
        Assigned?.FreeLocalNames();
    }

    public static void ParseIf(ASTNode node)
    {
        if (node.TryGetBrother<IfNode>(out var ifNode, -1))
            ifNode.SetElseNode(node);
        else throw new ExpectedException("if or elif", node);
    }
}

public class ElifNode : IfNode, IKeywordNode
{
    public ElifNode() => Executable = false;

    public override void Load()
    {
        base.Load();
        ElseNode.ParseIf(this);
    }
}

/// <summary>
/// Creates for loop from 0 to (value)
/// </summary>
public class RepeatNode() : ASTNode, IKeywordNode
{
    private IReturnAny? minNode;
    private IReturnAny? maxNode;
    private NameNode? nameNode;
    private BlockNode? blockNode;

    private BlockNode BlockNode => blockNode ?? throw new ExpectedException("{ }", this);

    public override void Load()
    {
        base.Load();
        minNode = GetNode<IReturnAny>(0);
        maxNode = GetNode<IReturnAny>(1);
        nameNode = GetNodeOrDefalut<NameNode>(2);
        blockNode = GetNode<BlockNode>(nameNode is null? 2 : 3);
    }

    public override void Execute()
    {
        base.Execute();
        
        int min = (int)(minNode?.AnyValue ?? 0);
        int max = (int)(maxNode?.AnyValue ?? 0);

        Variable var = new(nameNode?.Token.StringValue ?? string.Empty, min);

        if(var.Key != string.Empty)
        {
            Ugh.RegisterName(var);
            BlockNode.LocalNames.Add(var);
        }

        Ugh.EnterState(BlockNode);

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
        Ugh.QuitState();

        void Do(int i)
        {
            var.Value = i;
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

        Ugh.EnterState(Block);
        Block.Executable = true;
        
        while (Assigned.GetAny<bool>()) // TODO: Change this 
        {
            if (!Block.Executable) break;
            Block.Execute();
        }

        Block.Executable = false;
        Block.FreeLocalNames();
        Ugh.QuitState();
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
        itemNode = GetNode<NameNode>(0); // idk why this node can work without INamed but who cares
        collectionNode = GetNode<NameNode>(1);
        blockNode = GetNode<BlockNode>(2);
    }
   
    public override void Execute()
    {
        base.Execute();
        
        Ugh.EnterState(blockNode!);

        var collection = collectionNode?.AnyValue as IList<object> ?? throw new UghException($"Can not convert {collectionNode?.Token.StringValue} with value {collectionNode?.AnyValue} to array");
        
        Variable item = new(itemNode!.Token.StringValue, 0);
        Ugh.RegisterName(item);

        blockNode!.LocalNames.Add(item);

        blockNode.Executable = true;

        foreach (var obj in collection)
        {
            item.Value = obj;
            blockNode.Execute();
        }
        
        blockNode.Executable = false;
        blockNode.FreeLocalNames();

        Ugh.QuitState();
    }
}

/// <summary>
/// Imports other ugh files (and mark them as inserted) or loads modules
/// </summary>
public class InsertNode : ASTNode, IKeywordNode, INaming
{
    public override void Execute()
    {
        base.Execute();

        switch (GetNodeAt(0))
        {
            case ConstStringValueNode strValNode:
                var path = strValNode.Value;

                if(Directory.Exists(path)) { path += "\\master.ugh"; }
                else if (!File.Exists(path)) 
                    throw new FileNotFoundException(path);

                var file = File.ReadAllText(path);

                var parser = new Parser(Ugh, true, Parser.NoLoad);
                _ = new Lexer(file, parser);

                parser.Execute();
                break;
            case NameNode nn:
                var strNode = nn.Token.StringValue;

                var asNode = SearchForNode<AsNode>();
                var fromNode = SearchForNode<FromNode>();

                var name = asNode is null ? strNode : asNode.StringName;
                var fullName = string.IsNullOrEmpty(name) ? string.Empty : name + '.';

                var assembly = fromNode?.ConstAssembly.Assembly;
                assembly ??= Ugh.CurrentAssembly;

                var module = ModuleLoader.LoadModule(strNode, assembly);

                foreach (var method in module.Methods)
                {
                    ModuleFunction function = new($"{fullName}{method.Key}", Ugh, method.Value);
                    Ugh.RegisterName(function);
                }

                foreach (var field in module.Fields)
                {
                    Constant con = new($"{fullName}{field.Key}", field.Value.GetValue(null) ?? 0);
                    Ugh.RegisterName(con);
                }
                break;
        }
    }
}

/// <summary>
/// Prevents children from being inserted
/// /// </summary>
public class LocalNode : ASTNode, ITag, IKeywordNode
{
    public override void Load()
    {
        if (Parser.Inserted) { Executable = false; return; }

        if (TryGetNode<BlockNode>(0, out var tag)) // Nested local
            tag.Executable = true;

        base.Load(); // Load other things 
    }
}

public class AssemblyNode : ASTNode, IKeywordNode
{
    public override void Execute()
    {
        base.Execute();

        var pathNode = GetNode<ConstStringValueNode>(0);
        var asNode = GetNode<AsNode>(1);

        var assembly = ModuleLoader.LoadAssembly(pathNode.Value);

        var name = new AssemblyConst(asNode.StringName, assembly);
        Ugh.RegisterName(name);
    }
}

public class AsNode : ASTNode, INaming, IKeywordNode // TODO: Add converting types
{
    public string StringName 
    {
        get
        {
            return GetNodeAt(0) switch
            {
                NameNode nn => nn.Token.StringValue,
                ConstNullValueNode => string.Empty,
                _ => throw new ExpectedException("name or null", this),
            };
        }
    }
}

public class FromNode : AssignedNode<NameNode>, INaming, IKeywordNode
{
    public AssemblyConst ConstAssembly => Assigned.Name.GetAs<AssemblyConst>(); 
}


public class DefineNode : ASTNode, INaming, IKeywordNode, IReturnAny, IOperable
{
    private string name = string.Empty;
    private bool isdef;

    public object AnyValue => Ugh.IsDefined(name);

    public override void Load()
    {
        base.Load();
        switch (GetNodeAt(0))
        {
            case NameNode nn:
                name = nn.Token.StringValue;
                break;
            case ExpressionNode exn:
                name = exn.GetNode<NameNode>(0).Token.StringValue;
                isdef = true;
                break;
            default: throw new InvalidSpellingException(this);
        }
    }

    public override void Execute()
    {
        base.Execute();

        if(!isdef) 
            Ugh.RegisterName(new Constant(name, GetNodeOrDefalut<IReturnAny>(1)?.AnyValue ?? null!));
    }
}
