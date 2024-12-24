namespace UghLang;

public class Lexer
{
    public Parser Parser { get; }

    private string currentPart = string.Empty;
    private bool InsideString { get; } = false;
    private bool InsideComment { get; } = false;

    public Lexer(string contents, Parser parser)
    {
        Parser = parser;

        for (int i = 0; i < contents.Length; i++)
        {
            char c = contents[i];

            // comments
            if (c == '#' && !InsideString) {
                InsideComment = !InsideComment;
                continue;
            }
            if (InsideComment) continue;

            if (c == '"')
            {
                if(!IsPartEmpty() && !InsideString) AddPart(TokenType.Name);

                InsideString = !InsideString;
                if (!InsideString) AddPart(TokenType.StringValue);
            }
            else if (InsideString)
            {
                AddChar(c);
                continue;
            }
            else if (char.IsWhiteSpace(c))
            {
                // check if current part is not empty space
                if (IsPartEmpty()) continue;

                AddPart(TokenType.Name);
            }
            else if (c == ';' ) AddSingle(TokenType.Separator);

            else if (c == '(' ) AddSingle(TokenType.OpenExpression);
            else if (c == ')' ) AddSingle(TokenType.CloseExpression);

            else if (c == '{' ) AddSingle(TokenType.OpenBlock);
            else if (c == '}' ) AddSingle(TokenType.CloseBlock);

            else if (c == '[' ) AddSingle(TokenType.OpenList);
            else if (c == ']' ) AddSingle(TokenType.CloseList);

            else if (c == ',' ) AddSingle(TokenType.Comma);
            else if (c == 'π') AddSingle(TokenType.Pi);

            else if (char.IsDigit(c) || c == '-' && char.IsDigit(CheckNext()))
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
                    else if (ch == '.')
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
            else if (c.IsOperator())
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
    /// Saves last part and makes space for new one.
    /// </summary>
    /// <param name="type">Type of last token to seal</param>
    private void AddPart(TokenType type)
    {
        Token token = new(currentPart, type);
        currentPart = string.Empty;
        Parser.AddToken(token);
    }
}