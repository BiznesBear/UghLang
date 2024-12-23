namespace UghLang.Modules;

[Module("Time")]
public static class TimeModule
{
    public static void Sleep(float seconds) => Thread.Sleep((int)(seconds*1000));

    public static DateTime Now() => DateTime.Now;
    public static DateTime UtcNow() => DateTime.UtcNow;

    public static int ParseMillisecond(DateTime time) => time.Millisecond;
    public static int ParseSecond(DateTime time) => time.Second;
    public static int ParseMinute(DateTime time) => time.Minute;
    public static int ParseDay(DateTime time) => time.Day;
    public static int ParseDayOfYear(DateTime time) => time.DayOfYear;
    public static int ParseDayOfWeek(DateTime time) => (int)time.DayOfWeek;
    public static int ParseMonth(DateTime time) => time.Month;
    public static int ParseYear(DateTime time) => time.Year;

}
