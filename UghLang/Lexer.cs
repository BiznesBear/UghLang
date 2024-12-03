namespace UghLang;


public class Lexer
{
    public List<Token> Tokens => tokens;

    private List<Token> tokens = new();
    private string currentPart = string.Empty;
    private bool insideString = false;
    private bool insideComment = false;
    private bool ignore => insideString || insideComment;

    public Lexer(string contents)
    {
        for (int i = 0; i < contents.Length; i++)
        {
            char c = contents[i];

            // comments
            if (c == '#' && !insideString) {
                insideComment = !insideComment;
                continue;
            }
            if (insideComment) continue;



            if (c == '"' || c== '\'')
            {
                insideString = !insideString;
                if (!insideString) AddPart(TokenType.StringValue);
            }
            else if (char.IsWhiteSpace(c) && !ignore)
            {
                // check if current part is not empty space
                if(IsPartEmpty()) continue;
                
                AddPart(TokenType.None);
            }
            else if (c == ';' && !ignore)
            {
                if(!IsPartEmpty()) AddPart(TokenType.None);

                AddChar(c);
                AddPart(TokenType.Separator);
            }
            else if (char.IsDigit(c) && !ignore)
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
            else if (IsOperator(c) && !ignore)
            {
                if (!IsPartEmpty()) AddPart(TokenType.None);

                AddChar(c);

                char next = CheckNext();
                if (IsOperator(next)) { AddChar(next); Skip(); }

                AddPart(TokenType.Operator);
            }
            else AddChar(c);


            void Skip()
            {
                if (i + 1 < contents.Length)
                    i++;
            }
            char CheckNext()
            {
                if (i + 1 < contents.Length)
                    return contents[i + 1];
                return '\0';
            }
        }

        // for testing only
        Console.WriteLine(string.Join("\n", tokens));        
    }
 

    private bool IsPartEmpty() => currentPart == string.Empty;
    private void AddChar(char c) => currentPart += c;
    private void AddPart(TokenType type)
    {
        // save last part
        Token token = new(currentPart, type);
        tokens.Add(token);

        // and clear current
        currentPart = string.Empty;
    }
    public static bool IsOperator(char c) => c == '=' || c == '+' || c == '-' || c == '*' || c == '/';
}