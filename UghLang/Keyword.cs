using UghLang.Nodes;

namespace UghLang;
public enum Keyword : byte
{
    Print,
    Input,

    Free,

    True,
    False,

    Fun,
    Break,
    Return,

    If,
    Else,
    Elif,

    Repeat,
    While,
    Foreach,

    Insert,
    Local,

    Module,
    Assembly,
    
    As,
    From,

    Null,
    Def,
}

public static class KeywordExtensions
{
    private static readonly Dictionary<string, Keyword> Keywords = new()
    {
        { "print", Keyword.Print },
        { "input", Keyword.Input },

        { "free", Keyword.Free },

        { "true", Keyword.True },
        { "false", Keyword.False },

        { "fun", Keyword.Fun },
        { "break", Keyword.Break },
        { "return", Keyword.Return },

        { "if", Keyword.If },
        { "else", Keyword.Else },
        { "elif", Keyword.Elif },

        { "repeat", Keyword.Repeat },
        { "while", Keyword.While },
        { "foreach", Keyword.Foreach },

        { "insert", Keyword.Insert },
        { "local", Keyword.Local },

        { "module", Keyword.Module },
        { "assembly", Keyword.Assembly },
        
        { "as", Keyword.As },
        { "from", Keyword.From },

        { "null", Keyword.Null },
        { "def", Keyword.Def },
    };

    public static bool TryGetKeyword(this string word, out Keyword keyword, out TokenType type)
    {
        if (Keywords.TryGetValue(word, out Keyword value))
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

            Keyword.True => new ConstBoolValueNode() { Value = true },
            Keyword.False => new ConstBoolValueNode() { Value = false },

            Keyword.Fun => new DeclareFunctionNode(),
            Keyword.Break => new BreakNode(),
            Keyword.Return => new ReturnNode(),

            Keyword.If => new IfNode(),
            Keyword.Else => new ElseNode(),
            Keyword.Elif => new ElifNode(),

            Keyword.Repeat => new RepeatNode(),
            Keyword.While => new WhileNode(),
            Keyword.Foreach => new ForeachNode(),

            Keyword.Insert => new InsertNode(),
            Keyword.Local => new LocalNode(),

            Keyword.Module => new ModuleNode(),
            Keyword.Assembly => new AssemblyNode(),

            Keyword.As => new AsNode(),
            Keyword.From => new FromNode(),

            Keyword.Null => new ConstNullValueNode(),
            Keyword.Def => new DefineNode(),

            _ => new UndefinedNode(),
        };
    }
}
