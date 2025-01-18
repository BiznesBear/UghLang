namespace UghLang.Modules;

[Module("Environment")]
public static class EnvironmentModule
{
    public static string GetMachineName() => Environment.MachineName;
    public static string GetProcessPath() => Environment.ProcessPath ?? string.Empty;
    public static int GetProcessId() => Environment.ProcessId;
    public static void Throw(string message) => throw new SilentUghException(message);
    public static void Exit(int exitCode) => Environment.Exit(exitCode);
    public static void Clear() => Console.Clear();
}