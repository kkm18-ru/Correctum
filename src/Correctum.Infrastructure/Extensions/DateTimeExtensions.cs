namespace Correctum.Extensions;

public static class DateTimeExtensions
{
    extension(DateTime)
    {
        public static DateTime FromUnixTimeSeconds(long unixTimestampSeconds) =>
            DateTimeOffset.FromUnixTimeSeconds(unixTimestampSeconds).DateTime;
    }
}
