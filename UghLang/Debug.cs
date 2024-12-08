namespace UghLang;

public static class Debug
{
    public static void Print(object message)
    {
        #if DEBUG
        Console.WriteLine(message);
        #endif
    }
}
