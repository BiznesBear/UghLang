using UghLang.Nodes;
namespace UghLang;

public class Parser : IDisposable
{
    public readonly AST Ast;
    public bool Inserted { get; }

    private ASTNode currentNode;
    private Stack<ASTNode> masterBranches = new();

    public Parser(Ugh ugh, bool inserted = false)
    {
        Ast = new(ugh, this);
        currentNode = Ast;
        Inserted = inserted;
    }

    public void AddToken(Token token)
    {
        // Debug.Print(token);
        switch (token.Type)
        {
            case TokenType.Keyword:
                EnterNode((token.Keyword ?? default).GetNode());
                break;
            case TokenType.OpenExpression:
                EnterNode(new ExpressionNode()); 
                break;
            case TokenType.CloseExpression:
                QuitNode<IOperable>();
                QuitNode<ExpressionNode>();
                break;

            case TokenType.Operator:
                QuitNode<IOperable>();
                CreateNode(new OperatorNode() { Operator = token.Operator });
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

            case TokenType.OpenList:
                EnterNode(new ArrayNode());
                break;
            case TokenType.CloseList:
                QuitAllNodes<IOperable>();
                QuitNode<ArrayNode>();
                break;

            case TokenType.Name:
                ASTNode nameNode = IsMasterBranch() || currentNode is ITag? new NameNode() { Token = token } : new OperableNameNode() { Token = token };
                EnterNode(nameNode);
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
            case TokenType.Separator:
                BackToMasterBranch();
                break;
            case TokenType.Comma or TokenType.Colon: 
                QuitAllNodes<IOperable>();
                break;
            case TokenType.Pi:
                CreateNode(new ConstDoubleValueNode() { Value = (float)Math.PI });
                break;
            case TokenType.EOL:
                // Debug.Print(currentNode);
                if(currentNode is not IKeywordNode && currentNode is not NameNode) BackToMasterBranch();
                break;
            default: throw new UghException($"Unhandled token type: {token.Type}");
        }
    }


    #region NodeManagment

    /// <summary>
    /// Adds node to current node
    /// </summary>
    /// <param name="node"></param>
    private void CreateNode(ASTNode node) => currentNode.AddNode(node);

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
        if (currentNode.CheckType<T>())
        {
            QuitNode();
            return true;
        }
        return false;
    }
               
    private void QuitAllNodes<T>() { while (QuitNode<T>()); }


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
    public ASTNode GetMasterBranch() => masterBranches.Count < 1? Ast : masterBranches.Peek();

    /// <summary>
    /// Checks if current node is current masterBranch
    /// </summary>
    /// <returns></returns>
    private bool IsMasterBranch() => currentNode == GetMasterBranch();


    #endregion

    /// <summary>
    /// Loads AST 
    /// </summary>
    public void Load() => Ast.Load();

    /// <summary>
    /// Executes AST
    /// </summary>
    public void Execute() => Ast.Execute();

    /// <summary>
    /// Loads and executes AST
    /// </summary>
    public void LoadAndExecute()
    {
        Load();
        Execute();
    }

    public void Dispose() => GC.SuppressFinalize(this);
}
