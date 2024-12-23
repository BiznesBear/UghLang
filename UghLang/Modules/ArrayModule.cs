namespace UghLang.Modules;

[Module("Array")] // TODO: Introduce real arrays, and here add useful things
public static class ArrayModule // Yes i know. Very primitive. 
{
    public static object[] New(int bufferSize) => new object[bufferSize];
    public static object Get(object[] array, int index) => array[index];
    public static object[] Set(object[] array, int index, object obj)
    {
        array[index] = obj;
        return array;
    }
    public static int Lenght(object[] array) => array.Length;
}
