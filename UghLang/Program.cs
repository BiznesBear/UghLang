﻿using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json.Serialization;
using System.Text.Json;
using UghLang;
using UghLang.Nodes;

const string version = "v0.1";
const string ughlang = "UghLang " + version;


if (args.Length < 1) 
{
    Console.WriteLine(ughlang);
    return;
}

bool exe = true;
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
                Console.WriteLine("Welcome to Ugh language! Arguments list: --debug; --version; --help; --info; --nowarns; --stacktrace");
                break;
            case "--debug":
                Debug.EnabledMessages = true;
                break;
            case "--nowarns":
                Debug.EnabledWarrings = false;
                break;
            case "--stacktrace":
                Debug.EnabledStackTrace = true;
                break;
            case "--noexe":
                exe = false;
                break;
            default: throw new UghException($"Cannot find argument '{arg}'");
        }
    }
    else path = arg;
    
    string CheckNextArg(int next = 1)
    {
        int index = i + next;
        if (index < args.Length)
            return args[index];
        return string.Empty;
    }

    void SkipArg(int skips = 1)
    {
        if (i + skips < args.Length) i += skips;
    }
}

if (path == string.Empty) return;


var file = File.ReadAllText(path);
var ugh = new Ugh();
var parser = new Parser(ugh);

switch (Path.GetExtension(path))
{
    case ".ugh":
        var lexer = new Lexer(file, parser);
        Debug.PrintTree(parser.Ast, path);
        if (exe)
            parser.LoadAndExecute();
        break;
    case ".bin":
        Debug.Error("Binary AST is not currently supported");
        break;
    default: throw new UghException("Wrong file format: " + path);
}

