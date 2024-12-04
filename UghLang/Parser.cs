namespace UghLang;

public class Parser
{
    private readonly AST ast;

    public Parser(Lexer lex, Master m)
    {
        ast = new(m);

        var tokens = lex.Tokens;
        ASTNode? currentNode = null; 

        for (int i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];

            switch (token.Type)
            {
                case TokenType.None:
                    // check current node if null create refrence node
                    if (currentNode == null) currentNode = new RefrenceNode();
                    currentNode.AddToken(token);
                    break;
                case TokenType.Separator:
                    // end currentNode
                    ast.AddNode(GetCurrentNode());
                    ClearNode();
                    break;
                case TokenType.StringValue or TokenType.IntValue:
                    if (currentNode is not null)
                        currentNode.AddToken(token);
                    break;
                case TokenType.Keyword:
                    SetCurrentNode(token.Keyword ?? default);
                    break;
                case TokenType.Operator:
                    if (currentNode != null) currentNode.AddToken(token);
                    else throw new NullReferenceException("Null operation");
                    break;
                case TokenType.Comma:
                    // separate
                    break;
                case TokenType.OpenExpression:
                    // open expression node
                    break;
                case TokenType.CloseExpression:
                    // close expression node
                    break;
                case TokenType.OpenBlock:
                    // open code block node
                    break;
                case TokenType.CloseBlock:
                    // close code block node
                    break;
                default:
                    break;
            }
        }

            
        void ClearNode() => currentNode = null;


        void SetCurrentNode(Keyword keyword) 
        {
            currentNode = keyword switch
            {
                Keyword.Print => new PrintNode(),
                Keyword.Free => new FreeNode(),
                _ => null
            };
        }
        ASTNode GetCurrentNode() => currentNode ?? throw new Exception("No node attached");
    }
    public void Execute() => ast.Execute();
}

