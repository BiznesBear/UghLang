namespace UghLang;

public static class Debug
{
    public static bool Enabled { get; set; } = false;
    public static void Print(object message)
    {
        if (!Enabled) return;
        Console.WriteLine(message);
    }
}
