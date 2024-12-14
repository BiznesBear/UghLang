namespace UghLang.Nodes;

public abstract class ASTNode
{
    public const ASTNode NULL = default;

    #region Properties
    public bool CanExecute { get; set; } = true;
    public int CurrentIteration { get; private set; }

    private List<ASTNode> nodes = new();
    public IReadOnlyList<ASTNode> Nodes => nodes.AsReadOnly();

    private Ugh? ugh;
    public Ugh Ugh
    {
        get => ugh ?? throw new UghException("No Ugh in node");
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
        nodes.Add(node);
    }

    public bool HasEmptyBranch() => Nodes.Count < 1;

    #region GettingNodes

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


    public T? GetNodeWith<T>() where T : ASTNode
    {
        foreach (var n in Nodes)
        {
            if (CheckNodeType<T>(n))
                return (T)n;
        }
        return null;
    }

    public bool TryGetNodeWith<T>(out T node) where T : ASTNode
    {
        var n = GetNodeWith<T>();

        if (n is null)
        {
            node = (T)NULL;
            return false;
        }

        node = n;
        return true;
    }

    public IEnumerable<T> GetNodesWith<T>()
    {
        foreach (var n in Nodes)
            if (n is T t) yield return t;
    }

    public IEnumerable<T> GetAllNodesWith<T>()
    {
        foreach (var n in Nodes)
        {
            if (n is T t) yield return t;
            foreach (var dyn in n.GetAllNodesWith<T>())
                yield return dyn;
        }
    }
    #endregion


    /// <summary>
    /// Used for preloading nodes (to avoid it in execution)
    /// </summary>
    public virtual void Load()
    {
        for (int i = 0; i < Nodes.Count; i++)
            Nodes[i].Load();
    }

    /// <summary>
    /// Execute all assigned nodes.
    /// </summary>
    public virtual void Execute()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            if (CanExecute)
            {
                CurrentIteration = i;
                Nodes[i].Execute();
            }
        }
    }


    public override string ToString() => $"{GetType().Name}";
    public static bool CheckNodeType<T>(ASTNode node) => node.GetType() == typeof(T);
}


public class AST : ASTNode
{
    public AST(Ugh ugh)
    {
        Parent = this;
        Ugh = ugh;
    }
}

public class UndefinedNode : ASTNode
{
    public override void Execute()
    {
        base.Execute();
        throw new NotImplementedException("Undefined instructions");
    }
}
