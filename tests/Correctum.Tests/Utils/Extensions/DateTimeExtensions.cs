namespace Correctum.Tests.Utils.Extensions
{
    internal static class DateTimeExtensions
    {
        extension(DateTime dateTime)
        {
            public DateTime Round(long ticks) =>
                dateTime.AddTicks(-(dateTime.Ticks % ticks));
        }
    }
}
