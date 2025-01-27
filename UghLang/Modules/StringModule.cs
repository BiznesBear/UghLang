using System.Data;

namespace UghLang.Modules;

[Module("String")]
public static class StringModule
{
    public static int Lenght(string str) => str.Length;
    public static bool Contains(string str, string value) => str.Contains(value);
    public static string Join(object[] array, string separator) => string.Join(separator, array);
    public static string Trim(string text) => text.Trim();
    public static string TrimStart(string text) => text.TrimStart();
    public static string TrimEnd(string text) => text.TrimEnd();
    public static double Calc(string input)
    {
        string? back = new DataTable().Compute(input, null).ToString();
        bool can = int.TryParse(back, out int value);
        return can ? value : 0;
    }
}