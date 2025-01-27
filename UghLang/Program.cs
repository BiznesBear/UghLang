using System.Linq.Expressions;
using UghLang;



const string version = "v0.1dev";
const string ughlang = "UghLang " + version;
const string helpInfo = "Welcome to Ugh language! Arguments list: --debug; --version; --help; --info; --nowarns; --noload";

if (args.Length < 1) 
{
    Console.WriteLine(helpInfo);
    return;
}

bool noload = false;
string path = string.Empty;

for (var i = 0; i < args.Length; i++)
{
    var arg = args[i];
    
    if (arg.StartsWith("--"))
    {
        switch (arg)
        {
            case "--version":
                Console.WriteLine(ughlang);
                break;
            case "--info" or "--help":
                Console.WriteLine(helpInfo);
                break;
            case "--debug":
                Debug.EnabledMessages = true;
                break;
            case "--nowarns":
                Debug.EnabledWarrings = false;
                break;
            case "--noload":
                noload = true;
                break;
            default: throw new UghException($"Cannot find argument '{arg}'");
        }
    }
    else path = arg;
    
    string CheckNextArg(int next = 1)
    {
        int index = i + next;
        if (index < args.Length)
            return args[index];
        return string.Empty;
    }

    void SkipArg(int skips = 1)
    {
        if (i + skips < args.Length) i += skips;
    }
}

if (path == string.Empty) return;


var file = File.ReadAllText(path);
var ugh = new Ugh();
var parser = new Parser(ugh, false, noload);

switch (Path.GetExtension(path))
{
    case ".ugh" or ".txt":
        Debug.Print("Tokens:");
        new Lexer(file, parser);

        ugh.RegisterFile(path);

        Debug.PrintTree(parser.AST, path);
        Debug.Print("Output: ");

        if (!noload) parser.Execute();
        break;
    default: throw new UghException("Wrong file format: " + path);
}

