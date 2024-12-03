namespace UghLang;

public class Parser
{
    private AST ast;
    private Master master;

    public Parser(Lexer lex,Master m)
    {
        master = m;
        ast = new(master);

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
                    currentNode = null;
                    break;
                case TokenType.StringValue:
                    if(currentNode is not null)
                        currentNode.AddToken(token);
                    break;
                case TokenType.IntValue:
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
                default:
                    break;
            }
        }

        void SetCurrentNode(Keyword keyword)
        {
            currentNode = keyword switch
            {
                Keyword.Print => new PrintNode(),
                Keyword.Var => new VarNode(),
                Keyword.Free => new FreeNode(),
                _ => new PrintNode()
            };
        }
        ASTNode GetCurrentNode() => currentNode ?? throw new Exception("No node attached");
    }
    public void Execute() => ast.Execute();
}

