namespace UghLang.Modules;

[Module("Environment")]
public static class EnvironmentModule
{
    public static string GetMachineName() => Environment.MachineName;
    public static string NewLine() => Environment.NewLine;
    public static void Throw(string message, int exitCode)
    {
        Environment.ExitCode = exitCode;
        Debug.Ugh(message);
        Environment.Exit(exitCode);
    }
    public static void Exit(int exitCode) => Environment.Exit(exitCode);
    public static void Clear() => Console.Clear();
}