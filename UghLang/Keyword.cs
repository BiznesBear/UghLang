﻿using UghLang.Nodes;

namespace UghLang;
public enum Keyword
{
    Print,
    Input,

    Free,

    True,
    False,

    Fun,
    Break,
    If,
    Else,
    Elif,
    Repeat,
    Insert,
    Return
}
public static class KeywordExtension
{
    private readonly static Dictionary<string, Keyword> keywords = new()
    {
        { "print", Keyword.Print },
        { "input", Keyword.Input },
        { "free", Keyword.Free },
        { "true", Keyword.True },
        { "false", Keyword.False },
        { "fun", Keyword.Fun },
        { "break", Keyword.Break },
        { "if", Keyword.If },
        { "else", Keyword.Else },
        { "elif", Keyword.Elif },
        { "repeat", Keyword.Repeat },
        { "insert", Keyword.Insert },
        { "return", Keyword.Return },
    };

    public static bool TryGetKeyword(this string word, out Keyword keyword, out TokenType type)
    {
        if (keywords.TryGetValue(word, out Keyword value))
        {
            keyword = value;
            type = keyword == Keyword.True || keyword == Keyword.False ?
                TokenType.BoolValue : TokenType.Keyword;
            return true;
        }

        keyword = default;
        type = default;
        return false;
    }

    public static ASTNode GetNode(this Keyword keyword)
    {
        return keyword switch
        {
            Keyword.Print => new PrintNode(),
            Keyword.Input => new InputNode(),
            Keyword.Free => new FreeNode(),
            Keyword.True => new BoolValueNode() { Value = true },
            Keyword.False => new BoolValueNode() { Value = false },
            Keyword.Fun => new DeclareFunctionNode(),
            Keyword.Break => new BreakNode(),
            Keyword.If => new IfNode(),
            Keyword.Else => new ElseNode(),
            Keyword.Elif => new ElifNode(),
            Keyword.Repeat => new RepeatNode(),
            Keyword.Insert => new InsertNode(),
            Keyword.Return => new ReturnNode(),
            _ => new UndefinedNode(),
        };
    }
}
