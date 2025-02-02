namespace UghLang.Modules;

[Module("Environment")]
public static class EnvironmentModule
{
    public static string GetMachineName() => Environment.MachineName;
    public static void Throw(string message) => throw new UghException(message);
    public static void Exit(int exitCode) => Environment.Exit(exitCode);
    public static void Clear() => Console.Clear();
}