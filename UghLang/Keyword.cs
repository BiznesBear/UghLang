using UghLang.Nodes;

namespace UghLang;
public enum Keyword : byte
{
    Print,
    Input,

    Free,

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

    Def,
}

public static class KeywordExtensions
{
    private static readonly Dictionary<string, Keyword> Keywords = new()
    {
        { "print", Keyword.Print },
        { "input", Keyword.Input },

        { "free", Keyword.Free },

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

        { "def", Keyword.Def },
    };

    public static bool TryGetKeyword(this string word, out Keyword keyword) => Keywords.TryGetValue(word, out keyword);

    public static ASTNode GetNode(this Keyword keyword)
    {
        return keyword switch
        {
            Keyword.Print => new PrintNode(),
            Keyword.Input => new InputNode(),

            Keyword.Free => new FreeNode(),

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

            Keyword.Def => new DefineNode(),

            _ => new UndefinedNode(),
        };
    }
}
