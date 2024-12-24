namespace UghLang.Modules;

[Module("Convert")]
public static class ConvertModule
{
    private static T ConvertType<T>(object obj) => (T)Convert.ChangeType(obj, typeof(T));
    public static string String(object obj) => ConvertType<string>(obj);
    public static int Int(object obj) => ConvertType<int>(obj);
    public static bool Bool(object obj) => ConvertType<bool>(obj);
    public static float Float(object obj) => ConvertType<float>(obj);
    public static double Double(object obj) => ConvertType<double>(obj);
    public static long Long(object obj) => ConvertType<long>(obj);
    public static byte Byte(object obj) => ConvertType<byte>(obj);
}
