using UghLang;

const string version = "v0.1dev";
const string helpInfo = "Welcome to UghLang!\n Arguments list: \n--debug; --version; \n--help; --info; \n--nowarns; --noload";

if (args.Length < 1) 
{
    Console.WriteLine(helpInfo);
    return;
}

bool noexe = false;
string path = string.Empty;

for (var i = 0; i < args.Length; i++)
{
    var arg = args[i];
    
    if (arg.StartsWith("--"))
    {
        switch (arg)
        {
            case "--version":
                Console.WriteLine(version);
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
            case "--noexe":
                noexe = true;
                break;
            default: throw new UghException($"Cannot find argument '{arg}'");
        }
    }
    else path = arg;
}

if (path == string.Empty) 
    return;

var text = File.ReadAllText(path);

var ugh = new Rnm();
var parser = new Parser(ugh, false, noexe);

Debug.Print("Load:");
_ = new Lexer(text, parser);

Debug.PrintTree(parser.AST, path);
Debug.Print("Execute: ");

if (!noexe) 
    parser.Execute();
    
