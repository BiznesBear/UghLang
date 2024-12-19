namespace UghLang.Nodes;

public abstract class ASTNode
{
    public const ASTNode NULL = default;

    #region Properties

    public bool Executable { get; set; } = true;

    public IReadOnlyList<ASTNode> Nodes => nodes;
    public int CurrentIteration { get; private set; } = 0;

    private readonly List<ASTNode> nodes = new();

    private Parser? parser;
    public Parser Parser
    {
        get => parser ?? throw new UghException("No Parser assigned in node");
        set => parser = value;
    }

    private Ugh? ugh;
    public Ugh Ugh
    {
        get => ugh ?? throw new UghException("No Ugh assigned in node");
        set => ugh = value;
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
        node.Ugh = Ugh;
        node.Parser = Parser;
        nodes.Add(node);
    }


    #region GettingNodes

    public bool CheckType<T>() => this is T;
    public bool HasEmptyBranch() => Nodes.Count < 1;


    public T? GetNextBrother<T>() where T : ASTNode
    {
        var index = Parent.CurrentIteration + 1;
        if (index < Parent.Nodes.Count)
        {
            var n = Parent.Nodes[index];
            if (n is T t)
                return t;
        }
        return null;
    }

    public bool TryGetNextBrother<T>(out T node) where T : ASTNode
    {
        var index = Parent.CurrentIteration + 1;
        if (index < Parent.Nodes.Count)
        {
            var n = Parent.Nodes[index];
            if (n is T t)
            {
                node = t;
                return true;
            }
        }

        node = (T)NULL;
        return false;
    }

    public T? GetNodeOrDefalut<T>(int index) 
        => index < Nodes.Count && Nodes[index] is T t ? t : default;

    public T GetNode<T>(int index)
    {
        var node = GetNodeOrDefalut<T>(index);
        if(node?.GetType() == typeof(T))
            return node;
        throw new InvalidSpellingException(this);
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
        for (int i = 0; i < Nodes.Count; i++)
        {
            CurrentIteration = i;
            Nodes[i].Load();
        }
    }

    /// <summary>
    /// Executes all assigned nodes.
    /// </summary>
    public virtual void Execute()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            CurrentIteration = i;
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
    public AST(Ugh ugh, Parser parser)
    {
        Parent = this;
        Ugh = ugh;
        Parser = parser;
    }
}

public class UndefinedNode : ASTNode
{
    public override void Load() 
        => throw new NotImplementedException("Undefined instructions");
}
