using UghLang;

if (args.Length < 1) 
{
    Console.WriteLine("Ugh");
    return;
}

Debug.Enabled = true;

string path = string.Empty;
foreach(var arg in args)
{
    if (arg.StartsWith("--"))
    {
        switch (arg)
        {
            case "--version" or "--info" or "--help":
                // print version and general info
                break;
            case "--debug": // Enables debug
                Debug.Enabled = true; // TODO: Add error handling when is nothing to execute
                break;
            default: throw new ArgumentException(""); // ADD HERE UNDEFINED ARGUMENT EXCEPTION
        }
    }
    else path = arg;
}
if (path == string.Empty) return;

string file = File.ReadAllText(path);


var ugh = new Ugh();
var parser = new Parser(ugh);

Debug.Print("TOKENIZATION:\n");

var lexer = new Lexer(file, parser);

Debug.Print("\nPARSE:\n");

parser.Parse();

Debug.Print("\nEXECUTION:\n");

parser.Execute();