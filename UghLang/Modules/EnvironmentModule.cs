namespace UghLang.Modules;

[Module("Environment")]
public static class EnvironmentModule
{
    public static void Exit(int exitCode) => Environment.Exit(exitCode);
}