using System.Data;

namespace UghLang.Modules;

[Module("String")]
public static class StringModule
{
    public static int Lenght(string str) => str.Length;
    public static bool Contains(string str, string value) => str.Contains(value);
    public static string Join(string separator, object[] array) => string.Join(separator, array);

    public static double Calc(string input)
    {
        string? back = new DataTable().Compute(input, null).ToString();
        bool can = int.TryParse(back, out int value);
        return can ? value : 0;
    }
}