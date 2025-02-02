namespace UghLang.Nodes;

public delegate void ASTNodeEvent(ASTNode node);
public abstract class ASTNode
{
    public const ASTNode NULL = default;

    #region Properties

    public event ASTNodeEvent? NodeAdded;
    public int Position { get; set; } 
    public bool Executable { get; set; } = true;

    public IReadOnlyList<ASTNode> Nodes => nodes;

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
        NodeAdded?.Invoke(node);
    }


    #region GettingNodes

    public bool HasEmptyBranch() => Nodes.Count < 1;
    
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

    public T? GetNodeOrDefalut<T>(int index) 
        => index < Nodes.Count && Nodes[index] is T t ? t : default;

    public T GetNode<T>(int index)
    {
        var node = GetNodeOrDefalut<T>(index);
        if(node is not null)
            return node;
        throw new InvalidSpellingException(this);
    }

    public bool TryGetNode<T>(int index, out T node) 
    {
        var n = GetNodeOrDefalut<T>(index);

        if (n is null)
        {
            node = default!;
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


    public T? FindNode<T>() => GetNodes<T>().FirstOrDefault();
    public T? SearchForNode<T>() => GetAllNodes<T>().FirstOrDefault();

    #endregion


    /// <summary>
    /// Used for preloading nodes for faster execution time
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
    /// Executes all assigned nodes.
    /// </summary>
    public virtual void Execute()
    {
        for (int i = 0; i < Nodes.Count; i++)
            if (Executable)
                if (Nodes[i].Executable)
                    Nodes[i].Execute();
                else return;
    }

    public virtual void ForceExecute()
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
    public AST(Ugh ugh, Parser parser) 
    {
        Parent = this;
        Ugh = ugh;
        Parser = parser;
        NodeAdded += LoadNode;
    }

    private void LoadNode(ASTNode node)
    {
        if (Parser.NoLoad) return;

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
}

/// <summary>
/// Tags node as operable. Operable nodes can be quited by operators
/// </summary>
public interface IOperable;

/// <summary>
/// If parent is marked as INamed: NameNode will not try to parse name. Parser will only create NameNode instead of entering them.
/// </summary>
public interface INaming;

/// <summary>
/// Marks node as tag node. Tag node is recognized by parser ad master branch
/// </summary>
public interface ITag;

/// <summary>
/// Next node quits after parsing this node
/// </summary>
public interface IInstantQuit;
