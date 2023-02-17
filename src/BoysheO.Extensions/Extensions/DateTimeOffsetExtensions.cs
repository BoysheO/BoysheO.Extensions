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
        /// Get last Monday 0 AM.If v is Monday 0 am,return itself.<br />
        /// </summary>
        public static DateTimeOffset GetLastMonday0AM(this DateTimeOffset v)
        {
            var curDayOfWeek = v.DayOfWeek;
            var curDay = v.GetCurDay0Am();
            DateTimeOffset monDay = curDayOfWeek switch
            {
                DayOfWeek.Monday => curDay,
                DayOfWeek.Tuesday => curDay - TimeSpan.FromDays(1),
                DayOfWeek.Wednesday => curDay - TimeSpan.FromDays(2),
                DayOfWeek.Thursday => curDay - TimeSpan.FromDays(3),
                DayOfWeek.Friday => curDay - TimeSpan.FromDays(4),
                DayOfWeek.Saturday => curDay - TimeSpan.FromDays(5),
                DayOfWeek.Sunday => curDay - TimeSpan.FromDays(6),
                _ => throw new ArgumentOutOfRangeException()
            };
            return monDay;
        }

        /// <summary>
        /// Calculate the week day 0 am current week.<br />
        /// *Base on ISO 8601,week starts from Monday.It's very important.
        /// </summary>
        public static DateTimeOffset GetCurWeekDay0AM(this DateTimeOffset v, DayOfWeek dayOfWeek)
        {
            DateTimeOffset monDay = v.GetLastMonday0AM();
            return dayOfWeek switch
            {
                DayOfWeek.Monday => monDay,
                DayOfWeek.Tuesday => monDay.AddDays(1),
                DayOfWeek.Wednesday => monDay.AddDays(2),
                DayOfWeek.Thursday => monDay.AddDays(3),
                DayOfWeek.Friday => monDay.AddDays(4),
                DayOfWeek.Saturday => monDay.AddDays(5),
                DayOfWeek.Sunday => monDay.AddDays(6),
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
            };
        }

        /// <summary>
        /// Get current day 0 am.
        /// </summary>
        public static DateTimeOffset GetCurDay0Am(this DateTimeOffset v)
        {
            //使用除法运算求日期也不是不可以，long表示时间戳还有几年耗尽，DateTimeOffset也许会更新，那就按DateTimeOffset来运算吧！
            return new DateTimeOffset(v.Year, v.Month, v.Day, 0, 0, 0, v.Offset);
        }

        /// <summary>
        /// Get next day 0 am.
        /// 到下一天凌晨时间 （上午12点，也就是0点）
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static DateTimeOffset GetNextDay0Am(this DateTimeOffset t)
        {
            return t.GetCurDay0Am() + TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Get current hour 0min0sec.
        /// </summary>
        public static DateTimeOffset GetCurHour0(this DateTimeOffset v)
        {
            return new DateTimeOffset(v.Year, v.Month, v.Day, v.Hour, 0, 0, v.Offset);
        }

        /// <summary>
        /// Is the time is after now.<br />
        /// Useful to reduce mistake and make the code reading comfortable. 
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsExpire(this DateTimeOffset expire, DateTimeOffset now)
        {
            return expire < now;
        }

        /// <summary>
        /// Is the time is after <see cref="DateTimeOffset.Now"/>.<br />
        /// Useful to reduce mistake and make the code reading comfortable. 
        /// </summary>
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