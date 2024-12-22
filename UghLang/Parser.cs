using UghLang.Nodes;

namespace UghLang;

public class Parser
{
    public readonly AST AST;
    public bool Inserted { get; init; }

    private ASTNode currentNode;
    private Stack<ASTNode> masterBranches = new();

    public Parser(Ugh ugh, bool inserted = false)
    {
        AST = new(ugh, this);
        currentNode = AST;
        Inserted = inserted;
    }

    public void AddToken(Token token)
    {
        switch (token.Type)
        {
            case TokenType.Keyword:
                EnterNode((token.Keyword ?? default).GetNode());
                break;

            case TokenType.OpenExpression:
                EnterNode(new ExpressionNode()); 
                break;
            case TokenType.CloseExpression:
                QuitNode<ExpressionNode>();
                break;

            case TokenType.Operator:
                while (QuitNode<IOperatable>());
                CreateNode(new OperatorNode() { Operator = token.Operator });
                break;

            case TokenType.OpenBlock:
                EnterNode(new TagNode());
                CreateMasterBranch();
                break;
            case TokenType.CloseBlock:
                QuitNode<TagNode>();
                RemoveMasterBranch<TagNode>();
                break;

            case TokenType.OpenList:
                EnterNode(new ListNode());
                break;
            case TokenType.CloseList:
                QuitNode<ListNode>();
                break;

            case TokenType.Name:
                ASTNode nameNode = IsMasterBranch() ? new InitializeNode() { Token = token } : new NameNode() { Token = token };
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

    /// <summary>
    /// Loads AST 
    /// </summary>
    public void Load() => AST.Load();

    /// <summary>
    /// Executes AST
    /// </summary>
    public void Execute() => AST.Execute();

    /// <summary>
    /// Loads and executes AST
    /// </summary>
    public void LoadAndExecute()
    {
        Load();
        Execute();
    }
}

