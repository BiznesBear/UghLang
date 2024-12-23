using UghLang;


const string VERSION = "v0.2.2";

if (args.Length < 1) 
{
    Console.WriteLine("Ugh " + VERSION);
    return;
}

bool exe = true;
string path = string.Empty;
foreach(var arg in args)
{
    if (arg.StartsWith("--"))
    {
        switch (arg)
        {
            case "--version":
                Console.WriteLine($"UghLang {VERSION}");
                break;
            case "--info" or "--help":
                Console.WriteLine($"Welcome to Ugh language! Arguments list: --debug; --version; --help; --info");
                break;
            case "--debug": 
                Debug.Enabled = true; 
                break;
            case "--noexe":
                exe = false;
                break;
            default: throw new UghException($"Cannot find argument '{arg}'"); // ADD HERE UNDEFINED ARGUMENT EXCEPTION
        }
    }
    else path = arg;
}

if (path == string.Empty) return;

string file = File.ReadAllText(path);

var ugh = new Ugh();
var parser = new Parser(ugh, false);
var lexer = new Lexer(file, parser);

Debug.PrintTree(parser.AST, path);

if(exe) parser.LoadAndExecute();
