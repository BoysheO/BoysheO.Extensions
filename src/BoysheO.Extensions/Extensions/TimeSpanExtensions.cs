using System;
using System.Runtime.CompilerServices;

namespace BoysheO.Extensions
{
    public static class TimeSpanExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Clamp(this TimeSpan value, TimeSpan min, TimeSpan max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Max(this TimeSpan value, TimeSpan another)
        {
            if (value < another) return another;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan Min(this TimeSpan value, TimeSpan another)
        {
            if (value > another) return another;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TimeSpan MaxZero(this TimeSpan value)
        {
            return value.Max(TimeSpan.Zero);
        }

        /// <summary>
        /// 描述时间的大概长度
        /// ex:56d13h12m5s=>56d
        /// ex:0d13h12m5s=>13h
        /// ex:0d0h0m5s=>5s
        /// todo:make it low gc
        /// </summary>
        public static string ToSummaryText1(this TimeSpan value,string day,string hour,string minute,string second)
        {
            if (value.Days > 0) return value.Days + day;
            if (value.Hours > 0) return value.Hours + hour;
            if (value.Minutes > 0) return value.Minutes + minute;
            return value.Seconds + second;
        }
    }
}