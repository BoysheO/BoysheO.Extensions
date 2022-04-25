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
    }
}