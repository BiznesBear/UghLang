using UghLang;


if (args.Length < 1) 
{
    Console.WriteLine("Ugh");
    return;
}

string path = args[0];

string file = File.ReadAllText(path);
var ugh = new Ugh();
var lexer = new Lexer(file);
var parser = new Parser(lexer, ugh);

Debug.Print("\nEXECUTION:\n");

parser.Execute();
