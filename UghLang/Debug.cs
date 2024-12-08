namespace UghLang;

public static class Debug
{
    public static bool Enabled { get; set; } = true;
    public static void Print(object message)
    {
        if (!Enabled) return;

        #if DEBUG
        Console.WriteLine(message);
        #endif
    }
}
