namespace UghLang;

public class Lexer : IDisposable
{
    public Parser? Parser { get; }
    public Lexer(string input, Parser? parser = null) { Parser = parser; LexText(input); EndLexing(); }
    
    public IReadOnlyList<Token> Tokens => tokens;

    private readonly List<Token> tokens = new();
    private Token? lastToken;
    private bool insideString, insideComment;
    private string currentPart = string.Empty;

    public void LexText(string input)
    {
        for (int i = 0; i < input.Length; i++)
        {
            char c = input[i];

            // comments
            if (c == '#' && !insideString)
            {
                insideComment = !insideComment;
                continue;
            }

            if (insideComment) continue;

            if (c == '"')
            {
                if (!IsPartEmpty() && !insideString) AddPart(TokenType.Name);

                insideString = !insideString;
                if (!insideString) AddPart(TokenType.StringValue);
            }
            else if (insideString)
            {
                if(c == '\\')
                {
                    AddChar(CheckNext() switch
                    {
                        '\\' => '\\',
                        '\"' => '\"',
                        'n' => '\n',
                        't' => '\t',
                        '0' => '\0',
                        'b' => '\b',
                        'f' => '\f',
                        'e' => '\e',
                        'a' => '\a',
                        'v' => '\v',
                        _ => throw new UghException("Cannot find operand in string") // TODO: Change this message             
                    });
                    Skip();
                }
                else AddChar(c);
            }
            else if (char.IsWhiteSpace(c))
            {
                // check if current part is not empty space
                if (IsPartEmpty()) continue;
                AddPart(TokenType.Name);
            }
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

                    switch (ch)
                    {
                        case '.':
                            AddChar(ch);
                            Skip();
                            digitType = TokenType.FloatValue;
                            continue;
                        case '_':
                            Skip();
                            continue;
                        case 'f':
                            Skip();
                            digitType = TokenType.FloatValue;
                            break;
                        case 'd':
                            Skip();
                            digitType = TokenType.DoubleValue;
                            break;
                        case 'b':
                            Skip();
                            digitType = TokenType.ByteValue;
                            break;
                    }
                    break;
                }

                AddPart(digitType);
            }
            else if (BinaryOperation.IsOperator(c))
            {
                StartNew();
                AddChar(c);
                char next = CheckNext();
                if (BinaryOperation.IsOperator(next))
                {
                    AddChar(next);
                    Skip();
                }
                if (currentPart == "=>") AddPart(TokenType.Lambda);
                else if(currentPart == "!") AddPart(TokenType.Not);
                else AddPart(TokenType.Operator);
            }
            else switch (c)
            {
                case ';':
                    AddSingle(TokenType.Separator);
                    break;
                case '(':
                    AddSingle(TokenType.OpenExpression);
                    break;
                case ')':
                    AddSingle(TokenType.CloseExpression);
                    break;
                case '{':
                    AddSingle(TokenType.OpenBlock);
                    break;
                case '}':
                    AddSingle(TokenType.CloseBlock);
                    break;
                case '[':
                    AddSingle(TokenType.OpenIndex);
                    break;
                case ']':
                    AddSingle(TokenType.CloseIndex);
                    break;
                case ',':
                    AddSingle(TokenType.Comma);
                    break;
                case ':':
                    if (CheckNext() == ':')
                    {
                        StartNew();
                        AddChar(':');    
                        AddPart(TokenType.Colons);
                        Skip();
                    }
                    else AddSingle(TokenType.OpenExpression); // open expression
                    break;
                case '$':
                    AddSingle(TokenType.Preload);
                    break;
                case 'π':
                    AddSingle(TokenType.Pi);
                    break;
                default: 
                    AddChar(c);
                    break;
            }
            


            void Skip(int skips = 1)
            {
                if (i + skips < input.Length) i += skips;
            }

            char CheckNext(int next = 1)
            {
                int index = i + next;
                if (index < input.Length)
                {
                    char c = input[index];
                    if (char.IsWhiteSpace(c))
                        CheckNext(next + 1);
                    else
                        return c;
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
        
        if(!IsPartEmpty()) 
            AddPart(TokenType.Name);
    }
    
    public void EndLexing() => AddPart(TokenType.EndOfFile);
    
    private void StartNew()
    {
        if (!IsPartEmpty()) 
            AddPart(TokenType.Name);
    }

    private bool IsPartEmpty() => currentPart == string.Empty;
    private void AddChar(char c) => currentPart += c;
    private void AddPart(TokenType type)
    {
        // SPECIAL VALUE NODES
        type = currentPart switch
        {
            "true" or "false" => TokenType.BoolValue,
            "null" => TokenType.NullValue,
            _ => type
        };
        
        lastToken = new Token(currentPart, type);
        currentPart = string.Empty;
        tokens.Add(lastToken);
        Parser?.AddToken(lastToken);
    }
    public void Dispose() => GC.SuppressFinalize(this);
}
