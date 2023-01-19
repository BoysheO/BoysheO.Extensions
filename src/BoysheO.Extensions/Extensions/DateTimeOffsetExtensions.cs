using System;
using System.Runtime.CompilerServices;

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

        /// <summary>
        /// 算出当周的星期1的起点时间<br />
        /// *按ISO 8601规范，一周以周一为开始<br />
        /// </summary>
        public static DateTimeOffset GetLastMonday0AM(this DateTimeOffset v)
        {
            return GetCurWeekDay0AM(v, DayOfWeek.Monday);
        }

        /// <summary>
        /// 算出当周的星期n的开始时间
        /// *按ISO 8601规范，一周以周一为开始
        /// </summary>
        public static DateTimeOffset GetCurWeekDay0AM(this DateTimeOffset v, DayOfWeek dayOfWeek)
        {
            var curDayOfWeek = v.DayOfWeek;
            var curDay = v.GetCurDayStart();
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

            return dayOfWeek switch
            {
                DayOfWeek.Friday => monDay.AddDays(4),
                DayOfWeek.Monday => monDay,
                DayOfWeek.Saturday => monDay.AddDays(5),
                DayOfWeek.Sunday => monDay.AddDays(6),
                DayOfWeek.Thursday => monDay.AddDays(3),
                DayOfWeek.Tuesday => monDay.AddDays(1),
                DayOfWeek.Wednesday => monDay.AddDays(2),
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
            };
        }

        /// <summary>
        /// 到当天凌晨时间（上午12点，也就是0点）
        /// </summary>
        public static DateTimeOffset GetCurDayStart(this DateTimeOffset v)
        {
            return new DateTimeOffset(v.Year, v.Month, v.Day, 0, 0, 0, v.Offset);
        }

        /// <summary>
        /// 到下一天凌晨时间 （上午12点，也就是0点）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset GetNextDayStart(this DateTimeOffset t)
        {
            return t.GetCurDayStart() + TimeSpan.FromDays(1);
        }

        /// <summary>
        /// 到当小时0分
        /// </summary>
        public static DateTimeOffset GetCurHourStart(this DateTimeOffset v)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInRange(this DateTimeOffset now, DateTimeOffset start, DateTimeOffset end)
        {
            return now > start && now < end;
        }
    }
}