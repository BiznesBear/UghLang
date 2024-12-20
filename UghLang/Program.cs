﻿using UghLang;


const string VERSION = "v0.0.0.0.1";

if (args.Length < 1) 
{
    Console.WriteLine("Ugh " + VERSION);
    return;
}

#if DEBUG
Debug.Enabled = true;
#endif

string path = string.Empty;
foreach(var arg in args)
{
    if (arg.StartsWith("--"))
    {
        switch (arg)
        {
            case "--version" or "--info" or "--help":
                Console.WriteLine("Welcome to Ugh language " + VERSION);
                // print version and general info
                break;
            case "--debug": // Enables debug
                Debug.Enabled = true; // TODO: Add error handling when is nothing to execute
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

parser.LoadAndExecute();
