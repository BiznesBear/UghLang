namespace UghLang;
public enum Keyword
{
    Print,
    Free
}
public static class KeywordExtension
{
    private readonly static Dictionary<string, Keyword> keywords = new()
    {
        { "print", Keyword.Print },
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

    public static ASTNode GetNode(this Keyword keyword)
    {
        return keyword switch
        {
            Keyword.Print => new PrintNode(),
            Keyword.Free => new FreeNode(),
            _ => new UndefinedNode(),
        };
    }
}
