namespace Correctum.Utils.Extensions
{
    public static class DateTimeExtensions
    {
        extension(DateTime dt)
        {
            public long UnixTimestampMilliseconds => (dt.Ticks - 621355968000000000) / 10000;
            public long UnixTimestampSeconds => (dt.Ticks - 621355968000000000) / 10000000;
        }

        extension(DateOnly)
        {
            public static DateOnly OfdBeginDate => new(2016, 6, 1); 
        }
    }
}
