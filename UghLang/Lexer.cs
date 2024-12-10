namespace UghLang;


public class Lexer
{
    

    private string currentPart = string.Empty;
    private bool insideString = false;
    private bool insideComment = false;
    private bool Ignore => insideString || insideComment;
    private Parser Parser { get; }
    public Lexer(string contents, Parser parser)
    {
        Parser = parser;

        // WARRING: DO NOT CHANGE THE ORDER
        for (int i = 0; i < contents.Length; i++)
        {
            char c = contents[i];

            // comments
            if (c == '#' && !insideString) {
                insideComment = !insideComment;
                continue;
            }
            if (insideComment) continue;


            if (c == '"')
            {
                insideString = !insideString;
                if (!insideString) AddPart(TokenType.StringValue);
            }
            else if (char.IsWhiteSpace(c) && !Ignore)
            {
                // check if current part is not empty space
                if(IsPartEmpty()) continue;
                
                AddPart(TokenType.None);
            }
            else if (c == ';' && !Ignore) AddSingle(TokenType.Separator);
            else if (c == '(' && !Ignore) AddSingle(TokenType.OpenExpression);
            else if (c == ')' && !Ignore) AddSingle(TokenType.CloseExpression);
            else if (c == '{' && !Ignore) AddSingle(TokenType.OpenBlock);
            else if (c == '}' && !Ignore) AddSingle(TokenType.CloseBlock);
            else if ((char.IsDigit(c) || (c == '-' && char.IsDigit(CheckNext()))&& !Ignore))
            {
                AddChar(c);
                while (true)
                {
                    char ch = CheckNext();
                    if (char.IsDigit(ch))
                    {
                        AddChar(ch);
                        Skip();
                        continue;
                    }
                    else break; 
                }
                AddPart(TokenType.IntValue);
            }
            else if (c.IsOperator() && !Ignore)
            {
                StartNew();

                AddChar(c);

                char next = CheckNext();
                if (next.IsOperator()) { AddChar(next); Skip(); }

                AddPart(TokenType.Operator);
            }
            else AddChar(c);

            void Skip(int skips = 1)
            {
                if (i + skips < contents.Length)
                    i += skips;
            }
            char CheckNext(int next = 1)
            {
                if (i + next < contents.Length)
                    return contents[i + next];
                return '\0';
            }
            void AddSingle(TokenType type)
            {
                StartNew();
                AddChar(c);
                AddPart(type);
            }
        }

        Debug.Print(string.Join("\n", Parser.tokens));  // For testing only     
    }
    private void StartNew()
    {
        if (!IsPartEmpty()) 
            AddPart(TokenType.None);
    }
    private bool IsPartEmpty() => currentPart == string.Empty;
    private void AddChar(char c) => currentPart += c;



    /// <summary>
    /// Saves last part, and makes space for new one.
    /// </summary>
    /// <param name="type">Type of last token to seal</param>
    private void AddPart(TokenType type)
    {
        // save last part and clear current 
        Token token = new(currentPart, type);
        Parser.AddToken(token);
        currentPart = string.Empty;
    }

}