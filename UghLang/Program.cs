using UghLang;

string file = File.ReadAllText(@"C:\Users\ADAM\source\repos\UghLang\UghLang\master.ugh");
var ugh = new Master();
var lexer = new Lexer(file);
var parser = new Parser(lexer, ugh);
parser.Execute();

