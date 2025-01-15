﻿namespace UghLang.Nodes;


public abstract class ASTNode
{
    public const ASTNode NULL = default;

    #region Properties

    public IReadOnlyList<ASTNode> Nodes => nodes;

    public byte Iteration { get; private set; } = 0;
    public bool Executable { get; set; } = true;

    private readonly List<ASTNode> nodes = new();


    private ASTNode? parent;
    public ASTNode Parent
    {
        get => parent ?? this;
        set => parent = value;
    }


    private Ugh? ugh;
    public Ugh Ugh
    {
        get => ugh ?? throw new UghException("No Ugh assigned in node");
        set => ugh = value;
    }

    private Parser? parser;
    public Parser Parser
    {
        get => parser ?? throw new UghException("No Parser assigned in node");
        set => parser = value;
    }



    #endregion

    public void AddNode(ASTNode node)
    {
        node.Parent = this;
        node.Ugh = Ugh;
        node.Parser = Parser;
        nodes.Add(node);
    }


    #region GettingNodes

    public bool CheckType<T>() => this is T;
    public bool HasEmptyBranch() => Nodes.Count < 1;
    
    public T? GetNextBrother<T>() where T : ASTNode
    {
        var index = Parent.Iteration + 1;
        if (index < Parent.Nodes.Count)
        {
            var n = Parent.Nodes[index];
            if (n is T t)
                return t;
        }
        return null;
    }
    
    public ASTNode? GetNodeAt(int index) 
        => index < Nodes.Count? Nodes[index] : default;

    public T? GetNodeOrDefalut<T>(int index) 
        => index < Nodes.Count && Nodes[index] is T t ? t : default;

    public T GetNode<T>(int index)
    {
        var node = GetNodeOrDefalut<T>(index);
        if(node is not null)
            return node;
        throw new InvalidSpellingException(this);
    }
    
    public T HandleGetNode<T>(int index)
    {
        try { return GetNode<T>(index); }
        catch (Exception ex) { Debug.Error(ex); }
        return default!;
    }

    public bool TryGetNode<T>(int index, out T node) where T : ASTNode
    {
        var n = GetNodeOrDefalut<T>(index);

        if (n is null)
        {
            node = (T)NULL;
            return false;
        }

        node = n;
        return true;
    }

    public IEnumerable<T> GetNodes<T>()
    {
        foreach (var n in Nodes)
            if (n is T t) yield return t;
    }

    public IEnumerable<T> GetAllNodes<T>()
    {
        foreach (var n in Nodes)
        {
            if (n is T t) yield return t;
            foreach (var dyn in n.GetAllNodes<T>())
                yield return dyn;
        }
    }

    #endregion

    
    /// <summary>
    /// Used for preloading nodes for faster execution time
    /// </summary>
    public virtual void Load()
    {
        for (byte i = 0; i < Nodes.Count; i++)
        {
            Iteration = i;
            Nodes[i].Load();
        }
    }

    /// <summary>
    /// Executes all assigned nodes.
    /// </summary>
    public virtual void Execute()
    {
        for (byte i = 0; i < Nodes.Count; i++)
        {
            Iteration = i;
            if (Executable)
                Nodes[i].Execute();
        }
    }

   
    public void LoadAndExecute()
    {
        for (byte i = 0; i < Nodes.Count; i++)
        {
            Iteration = i;
            Nodes[i].Load();
            if (Executable)
                Nodes[i].Execute();
        }
    }

    public void ForceExecute()
    {
        var startingState = Executable;
        Executable = true;
        Execute();
        Executable = startingState;
    }

    public override string ToString() => $"{GetType().Name}";
}


public class AST : ASTNode
{
    public AST() { Parent = this; }
    public AST(Ugh ugh, Parser parser) : this()
    {
        Ugh = ugh;
        Parser = parser;
    }
}

public class UndefinedNode : ASTNode
{
    public override void Load() => throw new UndefinedInstructions();
}

/// <summary>
/// Tags node as operable 
/// </summary>
public interface IOperable;

/// <summary>
/// Prevents NameNode from being executed  
/// </summary>
public interface INamed;

/// <summary>
/// Marks node as tag node
/// </summary>
public interface ITag;