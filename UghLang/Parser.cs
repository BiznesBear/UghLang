namespace UghLang;

public class Parser
{
    public readonly AST AST;

    private ASTNode currentNode;
    private string treeString = string.Empty;

    public Parser(Lexer lex, Ugh ugh)
    {
        AST = new(ugh);
        currentNode = AST;
        var tokens = lex.Tokens;


        for (int i = 0; i < tokens.Count; i++)
        {
            Token currentToken = tokens[i];

            switch (currentToken.Type) 
            {
                case TokenType.Keyword:
                    ActionByKeyword(currentToken.GetSpecialValue().Keyword ?? default); 
                    break;
                case TokenType.Separator:
                    EndBranch();
                    // end branch
                    break;
                case TokenType.OpenExpression:
                    // add new expression
                    EnterNode(new ExpressionNode() { Dynamic = currentToken.Dynamic });
                    break;
                case TokenType.CloseExpression:
                    // exit and calculate current expression
                    QuitNode<ExpressionNode>();
                    break;
                case TokenType.StringValue or TokenType.IntValue or TokenType.Operator:
                    // add new value to expression
                    EnterNode(new DynamicNode() { Dynamic = currentToken.Dynamic });
                    break;
                case TokenType.None:
                    if (IsEmptyBranch())
                    {
                        var opr = CheckNext();
                        var val = CheckNext(2);
                        EnterNode(new InitVariableNode() { Token = currentToken, Operator = opr, ValueToken = val });
                        Skip();
                    }
                    else EnterNode(new RefrenceNode() { Token = currentToken,  });
                    break;
                default:
                    break; 
            }


            void Skip(int skips = 1)
            {
                if (i + skips < tokens.Count)
                    i += skips;
            }
            Token CheckNext(int next = 1)
            {
                if (i + next < tokens.Count)
                    return tokens[i + next];
                return Token.NULL;
            }
        }
    }

    private void EnterNode(ASTNode node)
    {
        currentNode.AddNode(node);
        currentNode = node;
        
        
        // FOR TESTING ONLY
        Debug.Print(treeString + node);
        treeString += "**";
    }
    private void QuitNode<T>()
    {
        if (CurrentNodeIs<T>())
        {
            
            currentNode = currentNode.Parent;
            treeString.Remove(treeString.Length - 1); // FOR TESTING ONLY
        }
    }
    private void EndBranch()
    {
        currentNode = AST;
        treeString = string.Empty;
    }
    private bool IsEmptyBranch() => currentNode == AST;
    private bool CurrentNodeIs<T>() => currentNode.GetType() == typeof(T);
    private bool TryGetNode<T>(out T? node) where T : ASTNode
    {
        node = null;
        if (CurrentNodeIs<T>())
        {
            node = (T)currentNode;
            return true;
        }
        else return false;
    }


    private void ActionByKeyword(Keyword keyword)
    {
        switch (keyword)
        {
            case Keyword.Print or Keyword.Free:
                // add node by keyword
                EnterNode(keyword.GetNode());
                break;
            default: break;
        }
    }

    public void Execute() => AST.Execute();
}

