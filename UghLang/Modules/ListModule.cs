namespace UghLang.Modules;


[Module("List")] 
public static class ListModule 
{
    public static List<object> New(object[] objects) => [.. objects];
    public static void Add(List<object> list, object obj) => list.Add(obj);
    public static bool Remove(List<object> list, object obj) => list.Remove(obj);
    public static void Clear(List<object> list) => list.Clear();
    public static int Count(List<object> list) => list.Count;
    public static int Capacity(List<object> list) => list.Capacity;
    public static void Insert(List<object> list, int index, object obj) => list.Insert(index, obj);
    public static void Contains(List<object> list, object obj) => list.Contains(obj);
}
