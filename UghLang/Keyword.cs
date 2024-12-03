﻿namespace UghLang;
public enum Keyword
{
    Print,
    Var,
    Free
}
public static class KeywordExtension
{
    private readonly static Dictionary<string, Keyword> keywords = new()
    {
        { "print", Keyword.Print },
        { "var", Keyword.Var },
        { "free", Keyword.Free }
    };
    public static bool TryGetKeyword(this string word, out Keyword? type)
    {
        if (keywords.TryGetValue(word, out Keyword value))
        {
            type = value;
            return true;
        }
        type = null;
        return false;
    }
}
