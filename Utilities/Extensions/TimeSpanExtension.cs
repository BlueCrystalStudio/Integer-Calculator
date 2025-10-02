namespace Utilities.Extensions;
public static class TimeSpanExtension
{
    public static string ToSecondsDisplayTime(this TimeSpan timespan) => timespan.Seconds.ToString() + "s " + timespan.Milliseconds.ToString() + "ms.";
}
