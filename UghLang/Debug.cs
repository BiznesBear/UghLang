using System.Text;
using UghLang.Nodes;

namespace UghLang;

public static class Debug
{
    public static bool EnabledMessages { get; set; } 
    public static bool EnabledWarrings { get; set; } = true;
    public static bool EnabledStackTrace { get; set; } = false;
    
    public static void Print(object message)
    {
        if (!EnabledMessages) return;
        Console.WriteLine(message);
    }
    
    public static void Warring(object message)
    {
        if (!EnabledWarrings) return;
        Console.WriteLine("Warring: " + message);
    }

    public static void Error(object message, int exitCode = 1)
    {
        Console.WriteLine("Error: " + message);
        Environment.Exit(exitCode);
    }
    
    public static void PrintTree(ASTNode node, string title = "DEBUG TREE") 
    { 
        Print($"\n{title}"); 
        Print(GenerateTreeString(node)); 
    }

    public static string GenerateTreeString(ASTNode node, string indent = "", bool isLast = true)
    {
        StringBuilder sb = new();
        sb.Append(indent);

        if (isLast)
        {
            sb.Append("└─ ");
            indent += "   ";
        }
        else
        {
            sb.Append("├─ ");
            indent += "│  ";
        }

        sb.AppendLine(node.ToString());

        for (int i = 0; i < node.Nodes.Count; i++)
            sb.Append(GenerateTreeString(node.Nodes[i], indent, i == node.Nodes.Count - 1));
        return sb.ToString();
    }
}
