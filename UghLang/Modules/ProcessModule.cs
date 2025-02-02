using System.Diagnostics;

namespace UghLang.Modules;

[Module("Process")]
public static class ProcessModule
{
    public static Process Start(string path, string? arguments) => arguments is null? Process.Start(path) : Process.Start(path, arguments);
    public static string GetProcessPath() => Environment.ProcessPath ?? string.Empty;
    public static int GetProcessId() => Environment.ProcessId;
}
