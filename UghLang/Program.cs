using UghLang;

const string version = "v0.1dev";
const string ughlang = "UghLang " + version;
const string helpInfo = "Welcome to UghLang!\n Arguments list: \n--debug; --version; \n--help; --info; \n--nowarns; --noload";

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
}

if (path == string.Empty) return;


var ugh = new Ugh();
var file = File.ReadAllText(path);
var parser = new Parser(ugh, false, noload);

switch (Path.GetExtension(path))
{
    case ".ugh":
        Debug.Print("Load:");

        _ = new Lexer(file, parser);
        Debug.PrintTree(parser.AST, path);

        Debug.Print("Execute: ");

        if (!noload) parser.Execute();
        break;
    default: throw new UghException("Wrong file format: " + path);
}
