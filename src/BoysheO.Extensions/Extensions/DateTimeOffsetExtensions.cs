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
        /// 到当天凌晨时间（上午12点，也就是0点）
        /// </summary>
        public static DateTimeOffset CurrentDay12Am(this DateTimeOffset v)
        {
            return new DateTimeOffset(v.Year, v.Month, v.Day, 0, 0, 0, v.Offset);
        }

        /// <summary>
        /// 到当小时0分
        /// </summary>
        public static DateTimeOffset CurrentHour0Min(this DateTimeOffset v)
        {
            return new DateTimeOffset(v.Year, v.Month, v.Day, v.Hour, 0, 0, v.Offset);
        }
    }
}