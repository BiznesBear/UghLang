namespace UghLang.Modules;

[Module("Array")] 
public static class ArrayModule 
{
    public static object[] New(int bufferSize) => new object[bufferSize];
    public static object[] Empty() => [];
    public static int Lenght(object[] array) => array.Length;
    public static object Contains(object[] array, object obj) => array.Contains(obj);
}
