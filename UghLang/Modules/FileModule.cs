namespace UghLang.Modules;

[Module("File")]
public static class FileModule
{
    public static string Read(string path) => File.ReadAllText(path);
    public static void Write(string path, string content) => File.WriteAllText(path, content);
    public static void Create(string path) => File.Create(path);
    public static void Delete(string path) => File.Delete(path);
    public static bool Exists(string path) => File.Exists(path);
}
