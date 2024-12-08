using UghLang;


if (args.Length < 1) 
{
    Console.WriteLine("Ugh");
    return;
}


Debug.Enabled = false;

string path = args[0];

string file = File.ReadAllText(path);
var ugh = new Ugh();
var parser = new Parser(ugh);
var lexer = new Lexer(file, parser);

Debug.Print("\nEXECUTION:\n");

parser.ParseAndExecute();