namespace Correctum.Utils.Extensions
{
    public static class DateTimeExtensions
    {
        extension(DateTime dt)
        {
            public long UnixTimestampTicks => dt.Ticks - DateTime.UnixEpochTicks;
            public long UnixTimestampMilliseconds => dt.UnixTimestampTicks / TimeSpan.TicksPerMillisecond;
            public long UnixTimestampSeconds => dt.UnixTimestampTicks / TimeSpan.TicksPerSecond;

            public DateTime Round(long ticks) => dt.AddTicks(-(dt.Ticks % ticks));
        }

        extension(DateTime)
        {
            public static long UnixEpochTicks => 621355968000000000;
            public static DateTime UnixEpoch => new(DateTime.UnixEpochTicks, 0);
            public static DateTime? FromUnixTimeSeconds(long? seconds) =>
                seconds is null ? null : DateTime.UnixEpoch.AddSeconds(seconds.Value);

            public static DateTime Max(DateTime value1, DateTime value2)
            {
                var ticksValue1 = value1.Ticks;
                var ticksValue2 = value2.Ticks;
                var ticksValue = Math.Max(ticksValue1, ticksValue2);
                return new(ticksValue, DateTimeKind.Unspecified);
            }

            public static DateTime Min(DateTime value1, DateTime value2)
            {
                var ticksValue1 = value1.Ticks;
                var ticksValue2 = value2.Ticks;
                var ticksValue = Math.Min(ticksValue1, ticksValue2);
                return new(ticksValue, DateTimeKind.Unspecified);
            }
        }

        extension(DateOnly)
        {
            public static DateOnly OfdBeginDate => new(2016, 6, 1); 
            public static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
            public static TimeSpan operator -(DateOnly left, DateOnly right) => TimeSpan.FromDays(left.DayNumber - right.DayNumber);
        }
    }
}
