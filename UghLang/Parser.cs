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
                // add new expression
                EnterNode(new ExpressionNode() ); // { AnyValue = token.Value }
                break;
            case TokenType.CloseExpression:
                // exit and calculate current expression
                QuitNode<ExpressionNode>();
                break;

            case TokenType.Operator:
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
                // end branch
                BackToMasterBranch();
                break;
            case TokenType.Name: // TODO: Rework this 
                if (IsMasterBranch()) EnterNode(new InitializeNode() { Token = token });
                else CreateNode(new NameNode() { Token = token });
                break;
            default: throw new Exception($"Unhandled token type: {token.Type}");
        }
    }


    #region NodeManagment

    private void CreateNode(ASTNode node) => currentNode.AddNode(node);

    /// <summary>
    /// Add node to currentNode that becomes currentNode
    /// </summary>
    /// <param name="node"></param>
    private void EnterNode(ASTNode node)
    {
        CreateNode(node);
        currentNode = node;
    }

    /// <summary>
    /// If the same type as currentNode then quits to parent of currentNode
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void QuitNode<T>()
    {
        if (CurrentNodeIs<T>())
            currentNode = currentNode.Parent;
    }

    /// <summary>
    /// Adds currentNode to masterBranches
    /// </summary>
    private void CreateMasterBranch() => masterBranches.Push(currentNode);


    /// <summary>
    /// Removes peek of masterBranches
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void RemoveMasterBranch<T>() // TODO: Fix debug tree in this function
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
    private bool CurrentNodeIs<T>() => currentNode.GetType() == typeof(T);


    #endregion

    public void Load() => AST.Load();
    public void Execute() => AST.Execute();

    /// <summary>
    /// Loads and executes every node
    /// </summary>
    public void LoadAndExecute()
    {
        Load();
        Execute();
    }
}
