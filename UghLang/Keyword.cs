namespace UghLang;
public enum Keyword
{
    Print,
    Free,
    True,
    False,
    Break,
    If,
    Repeat,
}
public static class KeywordExtension
{
    private readonly static Dictionary<string, Keyword> keywords = new()
    {
        { "print", Keyword.Print },
        { "free", Keyword.Free },
        { "true", Keyword.True },
        { "false", Keyword.False },
        { "break", Keyword.Break },
        { "if", Keyword.If },
        { "repeat", Keyword.Repeat },
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
            Keyword.Free => new FreeNode(),
            Keyword.True => new BoolValueNode() { Value = true },
            Keyword.False => new BoolValueNode() { Value = false },
            Keyword.Break => new BreakNode(),
            Keyword.If => new IfNode(),
            Keyword.Repeat => new RepeatNode(),
            _ => new UndefinedNode(),
        };
    }
}
