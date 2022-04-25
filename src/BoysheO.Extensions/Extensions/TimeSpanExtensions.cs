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
    }
}