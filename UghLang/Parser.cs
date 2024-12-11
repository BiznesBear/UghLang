namespace UghLang;

public class Parser
{
    public readonly AST AST;
    public readonly List<Token> tokens = new();

    private const string treeChar = "+--";

    private string treeString = string.Empty;
    private string recoveryTree = string.Empty;
    private ASTNode currentNode;
    private Stack<ASTNode> blocks = new();

    public Parser(Ugh ugh)
    {
        AST = new(ugh);
        currentNode = AST;
    }

    public AST Parse()
    {
        for (int i = 0; i < tokens.Count; i++) //TODO: Remove for loop and parse everything with new tokens from lexer 
        {
            Token currentToken = tokens[i];

            switch (currentToken.Type)
            {   
                case TokenType.Keyword:
                    ActionByKeyword(currentToken.Keyword ?? default);
                    break;

                case TokenType.OpenExpression:
                    // add new expression
                    EnterNode(new ExpressionNode() { Dynamic = currentToken.Dynamic });
                    break;
                case TokenType.CloseExpression:
                    // exit and calculate current expression
                    QuitNode<ExpressionNode>();
                    break;

                case TokenType.Operator:
                    CreateNode(new OperatorNode() { Operator = currentToken.Operator ?? default });
                    break;

                case TokenType.OpenBlock:
                    EnterNode(new TagNode());
                    GoNext();
                    break;
                case TokenType.CloseBlock:
                    QuitNode<TagNode>();
                    GoBack();
                    break;

                case TokenType.StringValue or TokenType.IntValue:
                    // add new value to expression
                    CreateNode(new DynamicNode() { Dynamic = currentToken.Dynamic });
                    break;
                case TokenType.Separator:
                    // end branch
                    GoDefalut();
                    break;
                case TokenType.None:
                    EnterNode(IsEmptyBranch()
                        ? new InitVariableNode() { Token = currentToken }
                        : new RefrenceNode() { Token = currentToken });
                    break;
                default:
                    Debug.Print("Somthing is unhandled");
                    break;
            }
        }
        return AST;
    }

    #region NodeManagment

    /// <summary>
    /// Adds new node
    /// </summary>
    /// <param name="node"></param>
    private void CreateNode(ASTNode node) { currentNode.AddNode(node); Debug.Print(treeString + node); }

    /// <summary>
    /// Creates and enters new node
    /// </summary>
    /// <param name="node"></param>
    private void EnterNode(ASTNode node)
    {
        CreateNode(node);
        recoveryTree = treeString;
        treeString += treeChar;
        currentNode = node;
    }

    private void QuitNode<T>()
    {
        if (CurrentNodeIs<T>())
        {
            currentNode = currentNode.Parent;
            treeString = treeString.Remove(treeString.Length - treeChar.Length,treeChar.Length); // FOR TESTING ONLY
        }
    }


    /// <summary>
    /// Go to last block (without removing item from top)
    /// </summary>
    private bool GoReset()
    {
        if (blocks.Count < 1) // Reset
        {
            currentNode = AST;
            treeString = string.Empty;
            return true;
        }
        return false;
    }

    /// <summary>
    /// Go to last block (without removing item from top)
    /// </summary>
    private void GoDefalut()
    {
        if (GoReset()) return; 
        currentNode = blocks.Peek();
        treeString = treeString.Remove(treeString.Length - treeChar.Length, treeChar.Length);
    }

    /// <summary>
    /// Create new block
    /// </summary>
    private void GoNext()
    {
        blocks.Push(currentNode);
    }

    /// <summary>
    /// Remove last block 
    /// </summary>
    private void GoBack()
    {
        for (int i = 0; i < 2; i++) // PLEASE DONT ASK WHAT THE HELL IS THIS
        {
            if (GoReset()) return;
            currentNode = blocks.Pop();
        }
    }

    private bool IsEmptyBranch() => currentNode == AST;
    private bool CurrentNodeIs<T>() => currentNode.GetType() == typeof(T);

    private void ActionByKeyword(Keyword keyword)
    {
        switch (keyword)
        {
            default: EnterNode(keyword.GetNode()); break;
        }
    }

    #endregion
    public void AddToken(Token token) => tokens.Add(token);
    public void Execute() => AST.Execute();
    public void ParseAndExecute()
    {
        Parse();
        Execute();
    }
}
