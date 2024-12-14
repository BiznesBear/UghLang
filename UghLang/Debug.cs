using System.Text;
using UghLang.Nodes;

namespace UghLang;

public static class Debug
{
    public static bool Enabled { get; set; } = false;
    public static void Print(object message)
    {
        if (!Enabled) return;
        Console.WriteLine(message);
    }
    public static void PrintTree(ASTNode node) 
    { 
        Print("\nDEBUG TREE"); 
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
