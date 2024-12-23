namespace UghLang.Modules;

[Module("File")]
public static class FileModule
{
    public static string Read(string path) => File.ReadAllText(path);
    public static void Write(string path, string content) => File.WriteAllText(path, content);
}
