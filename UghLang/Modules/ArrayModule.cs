namespace UghLang.Modules;

[Module("Array")] 
public static class ArrayModule 
{
    public static int Lenght(object[] array) => array.Length;
    public static object[] Empty() => Array.Empty<object>();
    public static object[] New(int bufferSize) => new object[bufferSize];
    
    public static object Get(object[] array, int index) => array[index];

    public static object[] Set(object[] array, int index, object obj)
    {
        array[index] = obj;
        return array;
    }
}
