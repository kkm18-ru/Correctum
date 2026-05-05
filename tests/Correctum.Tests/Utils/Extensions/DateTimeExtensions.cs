using System;
using System.Collections.Generic;
using System.Text;

namespace Correctum.Tests.Utils.Extensions
{
    internal static class DateTimeExtensions
    {
        extension(DateTime dateTime)
        {
            public DateTime Truncate(long ticks)
            {
                bool isValid = ticks == TimeSpan.TicksPerDay
                    || ticks == TimeSpan.TicksPerHour
                    || ticks == TimeSpan.TicksPerMinute
                    || ticks == TimeSpan.TicksPerSecond
                    || ticks == TimeSpan.TicksPerMillisecond;

                return isValid
                    ? dateTime.AddTicks(-(dateTime.Ticks % ticks))
                    : throw new ArgumentException();
            }
        }
    }
}
