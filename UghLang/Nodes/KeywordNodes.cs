﻿using NLua;
namespace UghLang.Nodes;

/// <summary>
/// Writes value 
/// </summary>
public class PrintNode : AssignedIReturnAnyNode 
{
    public override void Execute()
    {
        base.Execute();
        Console.WriteLine(any?.AnyValue);
    }
}

/// <summary>
/// Creates input field and writes value 
/// </summary>
public class InputNode : AssignedIReturnAnyNode, IReturn<string>
{
    public string Value { get; set; } = string.Empty;
    public object AnyValue => Value;

    public override void Execute()
    {
        base.Execute();
        Console.Write(any?.AnyValue);
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
/// Declares new function for Ugh
/// </summary>
public class DeclareFunctionNode : AssignedExpressionAndTagNode
{
    public DeclareFunctionNode() : base(1, 2) { }

    private NameNode? name;

    public override void Load()
    {
        base.Load();

        name = GetNode<NameNode>(0);

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
/// Sets current executed function value and breaks it whole execution
/// </summary>
public class ReturnNode : AssignedIReturnAnyNode, IReturnAny
{
    public Function Function => Ugh.Function ?? throw new InvalidSpellingException(this);
    public object AnyValue => Function.AnyValue;

    public override void Execute()
    {
        base.Execute();
            
        if(any is not null) 
            Function.Value = any.AnyValue;

        Function.TagNode.Executable = false;
    }
}

/// <summary>
/// Calls function and returns its value
/// </summary>
public class CallNode() : AssignedExpressionNode(1), IReturnAny, IQuitable
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

public class IfNode : AssignedExpressionAndTagNode
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
public class RepeatNode : AssignedExpressionAndTagNode
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
public class WhileNode : AssignedExpressionAndTagNode
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

public abstract class ConvertNode<T>(T defalutValue) : AssignedIReturnAnyNode, IReturn<T>, IQuitable
{
    public T Value { get; set; } = defalutValue;
    public object AnyValue => Value ?? throw new();

    public override void Execute()
    {
        base.Execute();
        Value = (T)Convert.ChangeType(Any.AnyValue, typeof(T));
    }
} 

public class StringNode() : ConvertNode<string>(string.Empty);
public class IntNode() : ConvertNode<int>(0);
public class BoolNode() : ConvertNode<bool>(false);
public class FloatNode() : ConvertNode<float>(0f);

public class ExternNode : ASTNode
{
    private ConstStringValueNode? script;
    public override void Load()
    {
        base.Load();
        script = GetNode<ConstStringValueNode>(0);
    }

    public override void Execute()
    {
        base.Execute();
        using var lua = new Lua();
        lua.DoString(File.ReadAllText(script?.Value ?? ""));
    }
}