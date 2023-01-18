using System;

namespace DateAndTime
{
    public readonly struct TimeOnlyOffset
    {
        public readonly TimeOnly TimeOnly;
        public readonly TimeSpan Offset;

        public TimeOnlyOffset(TimeOnly timeOnly, TimeSpan offset)
        {
            TimeOnly = timeOnly;
            Offset = offset;
        }

        public TimeOnlyOffset ToOffset(TimeSpan offset)
        {
            var span = TimeOnly.ToTimeSpan();
            var res = span - Offset + offset;
            var day = TimeSpan.FromDays(1);
            while (res < TimeSpan.Zero)
            {
                res += day;
            }

            while (res > day)
            {
                res -= day;
            }

            var timeOnly = new TimeOnly(res.Ticks);
            return new TimeOnlyOffset(timeOnly, offset);
        }

        public long ToUnixMillSec()
        {
            return ToOffset(TimeSpan.Zero).TimeOnly.Millisecond;
        }
    }
}