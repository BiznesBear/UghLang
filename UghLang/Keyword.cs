namespace UghLang;
public enum Keyword
{
    Print,
    Free,
    If,
    Repeat,
}
public static class KeywordExtension
{
    private readonly static Dictionary<string, Keyword> keywords = new()
    {
        { "print", Keyword.Print },
        { "free", Keyword.Free },
        { "if", Keyword.If },
        { "repeat", Keyword.Repeat },
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

    public static ASTNode GetNode(this Keyword keyword)
    {
        return keyword switch
        {
            Keyword.Print => new PrintNode(),
            Keyword.Free => new FreeNode(),
            Keyword.If => new IfNode(),
            Keyword.Repeat => new RepeatNode(),
            _ => new UndefinedNode(),
        };
    }
}
