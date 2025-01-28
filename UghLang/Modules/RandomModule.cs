namespace UghLang.Modules;

[Module("Random")] 
public static class RandomModule 
{
    public static int Next() => new Random().Next();
    public static int Range(int min, int max) => new Random().Next(min, max);
    public static double Double(double range) => new Random().NextDouble() * range;
    public static void Shuffle(object[] array) => new Random().Shuffle(array);
}
