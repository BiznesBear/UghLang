namespace UghLang;

public class Lexer
{
    public Parser Parser { get; }

    private string currentPart = string.Empty;
    private bool insideString = false;
    private bool insideComment = false;
    private bool Ignore => insideString || insideComment;

    public Lexer(string contents, Parser parser)
    {
        Parser = parser;

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
                
                AddPart(TokenType.Name);
            }
            else if (c == ';' && !Ignore) AddSingle(TokenType.Separator);
            
            else if (c == '(' && !Ignore) AddSingle(TokenType.OpenExpression);
            else if (c == ')' && !Ignore) AddSingle(TokenType.CloseExpression);
            
            else if (c == '{' && !Ignore) AddSingle(TokenType.OpenBlock);
            else if (c == '}' && !Ignore) AddSingle(TokenType.CloseBlock);

            else if (c == '[' && !Ignore) AddSingle(TokenType.OpenList);
            else if (c == ']' && !Ignore) AddSingle(TokenType.CloseList);
            
            else if (c == ',' && !Ignore) { /* Just do nothing here */ }

            else if ((char.IsDigit(c) || c == '-' && char.IsDigit(CheckNext())) && !Ignore)
            {
                AddChar(c);
                var digitType = TokenType.IntValue;
                while (true)
                {
                    char ch = CheckNext();
                    if (char.IsDigit(ch))
                    {
                        AddChar(ch);
                        Skip();
                        continue;
                    }
                    else if(ch == '.')
                    {
                        AddChar(ch);
                        Skip();
                        digitType = TokenType.FloatValue;
                        continue;
                    }
                    else if (ch == '_')
                    {
                        Skip();
                        continue;
                    }
                    else break; 
                }
                AddPart(digitType);
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
                int realIndex = i + next;
                if (realIndex < contents.Length)
                {
                    char c = contents[realIndex];
                    if (char.IsWhiteSpace(c)) CheckNext(next + 1);
                    else return c;
                }
                return '\0';
            }

            void AddSingle(TokenType type)
            {
                StartNew();
                AddChar(c);
                AddPart(type);
            }
        }
    }

    private void StartNew()
    {
        if (!IsPartEmpty()) 
            AddPart(TokenType.Name);
    }

    private bool IsPartEmpty() => currentPart == string.Empty;
    private void AddChar(char c) => currentPart += c;


    /// <summary>
    /// Saves last part, and makes space for new one.
    /// </summary>
    /// <param name="type">Type of last token to seal</param>
    private void AddPart(TokenType type)
    {
        Token token = new(currentPart, type);
        currentPart = string.Empty;

        Parser.AddToken(token);
    }
}