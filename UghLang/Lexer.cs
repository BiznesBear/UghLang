﻿namespace UghLang;

public class Lexer : IDisposable
{
    public Parser Parser { get; }

    private string currentPart = string.Empty;
    private bool insideString;
    private bool insideComment; 

    private Token? lastToken;


    /// <summary>
    /// Old way of lexing. Must use semicolons
    /// </summary>
    /// <param name="input">Text to lex</param>
    /// <param name="parser">Parser to send tokens</param>
    public Lexer(string input, Parser parser)  { Parser = parser; Lex(input); AddPart(TokenType.EOF); }

    private void Lex(string input)
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

            if (c == '"' || c == '\'')
            {
                if (!IsPartEmpty() && !insideString) AddPart(TokenType.Name);

                insideString = !insideString;
                if (!insideString) AddPart(TokenType.StringValue);
            }
            else if (insideString)
                AddChar(c);
            else if (char.IsWhiteSpace(c))
            {
                // check if current part is not empty space
                if (IsPartEmpty()) continue;
                AddPart(TokenType.Name);
            }
            
            else if (c == ';') AddSingle(TokenType.Separator);

            else if (c == '(') AddSingle(TokenType.OpenExpression);
            else if (c == ')') AddSingle(TokenType.CloseExpression);

            else if (c == '{') AddSingle(TokenType.OpenBlock);
            else if (c == '}') AddSingle(TokenType.CloseBlock);

            else if (c == '[') AddSingle(TokenType.OpenList);
            else if (c == ']') AddSingle(TokenType.CloseList);
            
            else if (c == ',') AddSingle(TokenType.Comma);
            else if (c == ':') AddSingle(TokenType.Colon);
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

                AddPart(TokenType.Operator);
            }
            else AddChar(c);

            
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
        if(!IsPartEmpty()) AddPart(TokenType.Name);
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
        lastToken = new(currentPart, type);
        currentPart = string.Empty;
        Parser.AddToken(lastToken);
    }
    public void Dispose() => GC.SuppressFinalize(this);
}
