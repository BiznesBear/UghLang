using UghLang;


const string version = "v0.2.2";
const string ughLangVersion = "UghLang " + version;

if (args.Length < 1) 
{
    Console.WriteLine(ughLangVersion);
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
                Console.WriteLine(ughLangVersion);
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
            default: throw new UghException($"Cannot find argument '{arg}'"); 
        }
    }
    else path = arg;
}

if (path == string.Empty) return;

var file = File.ReadAllText(path);

var ugh = new Ugh();
var parser = new Parser(ugh, false);
var lexer = new Lexer(file, parser);

Debug.PrintTree(parser.Ast, path);

if(exe) parser.LoadAndExecute();
