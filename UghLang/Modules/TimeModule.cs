namespace UghLang.Modules;

[Module("Time")]
public static class TimeModule
{
    public static void Sleep(double seconds) => Thread.Sleep((int)(seconds*1000));

    public static DateTime Now() => DateTime.Now;
    public static DateTime UtcNow() => DateTime.UtcNow;

    public static int ParseMillisecond(DateTime? time) => GetTime(time).Millisecond;
    public static int ParseSecond(DateTime time) => GetTime(time).Second;
    public static int ParseMinute(DateTime time) => GetTime(time).Minute;
    public static int ParseDay(DateTime time) => GetTime(time).Day;
    public static int ParseDayOfYear(DateTime time) => GetTime(time).DayOfYear;
    public static int ParseDayOfWeek(DateTime time) => (int)GetTime(time).DayOfWeek;
    public static int ParseMonth(DateTime time) => GetTime(time).Month;
    public static int ParseYear(DateTime time) => GetTime(time).Year;

    private static DateTime GetTime(DateTime? time) => time ?? DateTime.Now;
}
