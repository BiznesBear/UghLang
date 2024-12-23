namespace UghLang.Modules;

[Module("Math")]
public static class MathModule
{
    public static float Clamp(float val, float min, float max) => Math.Clamp(val, min, max);
}
