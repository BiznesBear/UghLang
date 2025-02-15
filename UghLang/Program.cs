using UghLang;

const string version = "v0.1dev";
const string helpInfo = "Welcome to UghLang!\n Arguments list: \n--debug; --version; \n--help; --info; \n--nowarns; --noload";

if (args.Length < 1) 
{
    Console.WriteLine(helpInfo);
    return;
}

bool onlyload = false;
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
            case "--onlyload":
                onlyload = true;
                break;
            default: throw new UghException($"Cannot find argument '{arg}'");
        }
    }
    else path = arg;
}

if (path == string.Empty) return;

var text = File.ReadAllText(path);

var ugh = new Rnm();
var parser = new Parser(ugh, false, onlyload);

Debug.Print("Load:");
_ = new Lexer(text, parser);

Debug.PrintTree(parser.AST, path);

Debug.Print("Execute: ");
if (!onlyload) 
    parser.Execute();