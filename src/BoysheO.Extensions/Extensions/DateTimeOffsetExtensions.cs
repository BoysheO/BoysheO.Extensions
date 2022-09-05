using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace BoysheO.Extensions
{
    public static class DateTimeOffsetExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset Clamp(this DateTimeOffset value, DateTimeOffset min, DateTimeOffset max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset Max(this DateTimeOffset value, DateTimeOffset another)
        {
            if (value < another) return another;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset Min(this DateTimeOffset value, DateTimeOffset another)
        {
            if (value > another) return another;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset MaxNow(this DateTimeOffset value)
        {
            return value.Max(DateTimeOffset.Now);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset MinNow(this DateTimeOffset value)
        {
            return value.Min(DateTimeOffset.Now);
        }

        /// <summary>
        /// 算出当周的星期1的0点时间
        /// *按ISO 8601规范，一周以周一为开始
        /// </summary>
        public static DateTimeOffset CurrentWeekMonday0AM(this DateTimeOffset v)
        {
            var curDayOfWeek = v.DayOfWeek;
            var curDay = v.CurrentDay12Am();
            DateTimeOffset monDay = curDayOfWeek switch
            {
                DayOfWeek.Friday => curDay - TimeSpan.FromDays(4),
                DayOfWeek.Monday => curDay,
                DayOfWeek.Saturday => curDay - TimeSpan.FromDays(5),
                DayOfWeek.Sunday => curDay - TimeSpan.FromDays(6),
                DayOfWeek.Thursday => curDay - TimeSpan.FromDays(3),
                DayOfWeek.Tuesday => curDay - TimeSpan.FromDays(1),
                DayOfWeek.Wednesday => curDay - TimeSpan.FromDays(2),
                _ => throw new ArgumentOutOfRangeException()
            };

            return monDay;
        }

        /// <summary>
        /// 算出当周的星期n的0点时间
        /// *按ISO 8601规范，一周以周一为开始
        /// </summary>
        public static DateTimeOffset CurrentWeekDay(this DateTimeOffset v, DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Friday => v.CurrentWeekMonday0AM().AddDays(4),
                DayOfWeek.Monday => v.CurrentWeekMonday0AM(),
                DayOfWeek.Saturday => v.CurrentWeekMonday0AM().AddDays(5),
                DayOfWeek.Sunday => v.CurrentWeekMonday0AM().AddDays(6),
                DayOfWeek.Thursday => v.CurrentWeekMonday0AM().AddDays(3),
                DayOfWeek.Tuesday => v.CurrentWeekMonday0AM().AddDays(1),
                DayOfWeek.Wednesday => v.CurrentWeekMonday0AM().AddDays(2),
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
            };
        }

        /// <summary>
        /// 到当天凌晨时间（上午12点，也就是0点）
        /// </summary>
        public static DateTimeOffset CurrentDay12Am(this DateTimeOffset v)
        {
            return new DateTimeOffset(v.Year, v.Month, v.Day, 0, 0, 0, v.Offset);
        }

        /// <summary>
        /// 到下一天凌晨时间 （上午12点，也就是0点）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset NextDay12Am(this DateTimeOffset t)
        {
            return t.CurrentDay12Am() + TimeSpan.FromDays(1);
        }

        /// <summary>
        /// 到当小时0分
        /// </summary>
        public static DateTimeOffset CurrentHour0Min(this DateTimeOffset v)
        {
            return new DateTimeOffset(v.Year, v.Month, v.Day, v.Hour, 0, 0, v.Offset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsExpire(this DateTimeOffset expire, DateTimeOffset now)
        {
            return expire < now;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsExpire(this DateTimeOffset expire)
        {
            return IsExpire(expire, DateTimeOffset.Now);
        }
    }
}