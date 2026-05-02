namespace Correctum.Utils.Extensions;

public static class DateOnlyExtensions
{
    extension(DateOnly)
    {
        public static DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
        public static TimeSpan operator -(DateOnly left, DateOnly right) => TimeSpan.FromDays(left.DayNumber - right.DayNumber);
    }
}
