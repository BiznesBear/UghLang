namespace UghLang.Modules;

[Module("Time")]
public static class TimeModule
{
    public static void Sleep(double seconds) => Thread.Sleep((int)(seconds*1000));

    public static DateTime Now() => DateTime.Now;
    public static DateTime UtcNow() => DateTime.UtcNow;

    public static long Ticks(DateTime? time) => GetTime(time).Ticks;
    public static int Millisecond(DateTime? time) => GetTime(time).Millisecond;
    public static int Second(DateTime? time) => GetTime(time).Second;
    public static int Minute(DateTime? time) => GetTime(time).Minute;
    public static int Day(DateTime? time) => GetTime(time).Day;
    public static int DayOfYear(DateTime? time) => GetTime(time).DayOfYear;
    public static int DayOfWeek(DateTime? time) => (int)GetTime(time).DayOfWeek;
    public static int Month(DateTime? time) => GetTime(time).Month;
    public static int Year(DateTime? time) => GetTime(time).Year;

    private static DateTime GetTime(DateTime? time) => time ?? DateTime.Now;
}
