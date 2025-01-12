namespace UghLang.Modules;

[Module("Random")] 
public static class RandomModule 
{
    public static int RandInt(int min, int max) => new Random().Next(min, max);
    public static double RangeDouble(double range) => new Random().NextDouble() * range;
    public static void Shuffle(object[] array) => new Random().Shuffle(array);
}
