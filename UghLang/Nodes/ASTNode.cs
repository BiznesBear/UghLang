namespace UghLang.Nodes;

public delegate void AstNodeEvent(ASTNode node);
public abstract class ASTNode
{
    public event AstNodeEvent? NodeAdded;
    public Rnm Rnm => Parser.Rnm;

    #region Properties

    public int Position { get; set; } 
    public bool Executable { get; set; } = true;

    public IReadOnlyList<ASTNode> Nodes => nodes;

    private readonly List<ASTNode> nodes = new();
    

    private Parser? parser;
    public Parser Parser
    {
        get => parser ?? throw new UghException("No Parser assigned in node");
        set => parser = value;
    }

    private ASTNode? parent;
    public ASTNode Parent
    {
        get => parent ?? this;
        set => parent = value;
    }


    #endregion

    public void AddNode(ASTNode node)
    {
        node.Parent = this;
        node.Parser = Parser;
        nodes.Add(node);
        NodeAdded?.Invoke(node);
    }


    #region GettingNodes
    
    public T? GetBrother<T>(int skips = 1) 
    {
        int index = Position + skips;
        if (index < Parent.Nodes.Count && index >= 0)
        {
            var n = Parent.Nodes[index];
            if (n is T t)
                return t;
        }
        return default;
    }

    public bool TryGetBrother<T>(out T node, int skips = 1) 
    {
        node = GetBrother<T>(skips) ?? default!;
        return node is not null;
    }

    public ASTNode? GetNodeAt(int index) 
        => index < Nodes.Count? Nodes[index] : default;

    public T? GetNodeOrDefault<T>(int index) 
        => index < Nodes.Count && Nodes[index] is T t ? t : default;

    public T GetNode<T>(int index)
    {
        var node = GetNodeOrDefault<T>(index);
        if(node is not null)
            return node;
        throw new ExpectedException(typeof(T).ToString(), this);
    }

    public bool TryGetNode<T>(int index, out T node) 
    {
        var n = GetNodeOrDefault<T>(index);

        if (n is null)
        {
            node = default!;
            return false;
        }

        node = n;
        return true;
    }

    /// <summary>
    /// Get nodes of specified type from assigned nodes
    /// </summary>
    /// <typeparam name="T">nodes type</typeparam>
    /// <returns>IEnumerable with T</returns>
    public IEnumerable<T> GetNodes<T>()
    {
        foreach (var n in Nodes)
            if (n is T t) yield return t;
    }

    /// <summary>
    /// Get nodes of specified type from assigned nodes and their children
    /// </summary>
    /// <typeparam name="T">nodes type</typeparam>
    /// <returns>IEnumerable with T</returns>
    public IEnumerable<T> GetAllNodes<T>()
    {
        foreach (var n in Nodes)
        {
            if (n is T t) yield return t;
            foreach (var dyn in n.GetAllNodes<T>())
                yield return dyn;
        }
    }

    public T? FindNode<T>() => GetNodes<T>().FirstOrDefault();
    public T? SearchForNode<T>() => GetAllNodes<T>().FirstOrDefault();

    #endregion


    /// <summary>
    /// Loads this and assigned nodes
    /// </summary>
    public virtual void Load()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            Nodes[i].Position = i;
            Nodes[i].Load();
        }
    }

    /// <summary>
    /// Executes this and assigned nodes
    /// </summary>
    public virtual void Execute()
    {
        foreach (var t in Nodes)
            if (Executable)
                if (t.Executable)
                    t.Execute();
                else return;
    }

    /// <summary>
    /// Force node to execute
    /// </summary>
    public void ForceExecute()
    {
        var startingState = Executable;
        Executable = true;
        Execute();
        Executable = startingState;
    }
    
    public override string ToString() => $"{GetType().Name}({Position})";
}


public class AST : ASTNode
{
    private ASTNode? previous;
    public AST(Parser parser) 
    {
        Parent = this;
        Parser = parser;
        NodeAdded += LoadNode;
    }

    private void LoadNode(ASTNode node)
    {
        if (Parser.OnlyLoad) return;

        if (previous is not null)
        {
            previous.Position = Position;
            previous.Load();
            Position++;
        }
        previous = node;
    }
}

public class UndefinedNode : ASTNode
{
    public override void Load() => throw new UndefinedInstructions();
    public override void Execute() => throw new UndefinedInstructions();
}

/// <summary>
/// Operators quits from IOperable nodes 
/// </summary>
public interface IOperableNode;

/// <summary>
/// NameNode children will not try to get name. Parser will create NameNode instead of entering them.
/// </summary>
public interface INamingNode;

/// <summary>
/// Tag node is recognized by parser as master branch
/// </summary>
public interface ITagNode;

/// <summary>
/// Parser quits this node after adding next node
/// </summary>
public interface INextQuitNode;
