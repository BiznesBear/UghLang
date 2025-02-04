using UghLang.Nodes;
namespace UghLang;

public class Parser : IDisposable
{
    public Ugh Ugh { get; }
    public AST AST { get; }
    public bool Inserted { get; }
    public bool NoLoad { get; }

    private readonly Stack<ASTNode> masterBranches = new();
    private ASTNode currentNode;
    

    public Parser(Ugh ugh, bool inserted, bool noload)
    {
        AST = new(this);
        currentNode = AST;
        Ugh = ugh;
        Inserted = inserted;
        NoLoad = noload;
    }

    public Parser(Ugh ugh, bool inserted, bool noload, IEnumerable<Token> tokens) : this(ugh, inserted, noload) => AddTokens(tokens);

    public void AddTokens(IEnumerable<Token> tokens)
    {
        foreach (var t in tokens)
            AddToken(t);
    }

    public void AddToken(Token token)
    {
        bool instaQuit = currentNode is IInstantQuit;

        switch (token.Type)
        {
            case TokenType.Name:
                if(token.StringValue.TryGetKeyword(out Keyword kw))
                {
                    EnterNode(kw.GetNode());
                    break;
                }

                ASTNode nameNode = IsMasterBranch() || currentNode is ITag ? new NameNode() { Token = token } : new OperableNameNode() { Token = token };
                if (currentNode is INaming) CreateNode(nameNode);
                else EnterNode(nameNode);

                break;
            case TokenType.OpenExpression or TokenType.Colon:
                EnterNode(new ExpressionNode()); 
                break;
            case TokenType.CloseExpression:
                QuitNode<IOperable>();
                QuitNode<ExpressionNode>();
                break;
            case TokenType.Operator:
                QuitNode<IOperable>();
                EnterNode(new OperatorNode() { Operator = token.Operator });
                break;
            case TokenType.OpenBlock:
                QuitNode<IOperable>();
                EnterNode(new BlockNode());
                CreateMasterBranch();
                break;
            case TokenType.CloseBlock:
                QuitNode<BlockNode>();
                RemoveMasterBranch<BlockNode>();
                break;
            case TokenType.OpenIndex:
                EnterNode(new IndexNode());
                break;
            case TokenType.CloseIndex:
                QuitNode<IOperable>();
                QuitNode<IndexNode>();
                break;
            case TokenType.StringValue:
                CreateNode(new ConstStringValueNode() { Value = token.StringValue });
                break;
            case TokenType.IntValue:
                CreateNode(new ConstIntValueNode() { Value = token.IntValue });
                break;
            case TokenType.BoolValue:
                CreateNode(new ConstBoolValueNode() { Value = token.BoolValue });
                break;
            case TokenType.FloatValue:
                CreateNode(new ConstFloatValueNode() { Value = token.FloatValue });
                break;
            case TokenType.DoubleValue:
                CreateNode(new ConstDoubleValueNode() { Value = token.DoubleValue });
                break;
            case TokenType.ByteValue:
                CreateNode(new ConstByteValueNode() { Value = token.ByteValue });
                break;
            case TokenType.NullValue:
                CreateNode(new ConstNullValueNode());
                break;
            case TokenType.Separator:
                BackToMasterBranch();
                break;
            case TokenType.Comma: 
                QuitNode<IOperable>();
                QuitNode<NameNode>();
                break;     
            case TokenType.Preload:
                EnterNode(new PreloadNode());
                break;
            case TokenType.Not:
                EnterNode(new NotNode());
                break;
            case TokenType.Pi:
                CreateNode(new ConstDoubleValueNode() { Value = Math.PI });
                break;
            case TokenType.Lambda:
                EnterNode(new BlockNode());
                break;  
            case TokenType.EndOfFile:
                AST.AddNode(new EndOfFileNode());
                break;
            default: throw new UghException($"Unhandled token type: {token.Type}");
        }

        if (instaQuit) QuitNode<IInstantQuit>();
    }


    #region NodeManagment

    /// <summary>
    /// Adds node to current node
    /// </summary>
    /// <param name="node"></param>
    private void CreateNode(ASTNode node)
    {
        currentNode.AddNode(node);
    }

    /// <summary>
    /// Add node to currentNode and sets it as new current node
    /// </summary>
    /// <param name="node"></param>
    private void EnterNode(ASTNode node)
    {
        CreateNode(node);
        currentNode = node;
    }

    private void QuitNode() => currentNode = currentNode.Parent;

    /// <summary>
    /// Quits from node T
    /// </summary>
    /// <typeparam name="T">Type of node</typeparam>
    private bool QuitNode<T>()
    {
        if (currentNode is T)
        {
            QuitNode();
            return true;
        }
        return false;
    }


    /// <summary>
    /// Adds currentNode to masterBranches
    /// </summary>
    private void CreateMasterBranch() => masterBranches.Push(currentNode);


    /// <summary>
    /// Removes peek of masterBranches
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void RemoveMasterBranch<T>() 
    {
        if (GetMasterBranch().GetType() != typeof(T)) return;
        masterBranches.Pop();
        currentNode = GetMasterBranch();
    }

    /// <summary>
    /// Set current node from peek of masterBranches
    /// </summary>
    private void BackToMasterBranch() => currentNode = GetMasterBranch();


    /// <summary>
    /// Returns current master branch
    /// </summary>
    public ASTNode GetMasterBranch() => masterBranches.Count < 1? AST : masterBranches.Peek();

    /// <summary>
    /// Checks if current node is current masterBranch
    /// </summary>
    /// <returns></returns>
    private bool IsMasterBranch() => currentNode == GetMasterBranch();


    #endregion

    public void Execute() => AST.Execute();

    public void Dispose() => GC.SuppressFinalize(this);
}
