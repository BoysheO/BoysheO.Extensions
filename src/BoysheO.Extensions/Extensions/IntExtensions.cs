using System;
using System.Runtime.CompilerServices;

namespace BoysheO.Extensions
{
    public static class IntExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Abs(this int i)
        {
            return Math.Abs(i);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(this int value, int min, int max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp01(this int value)
        {
            return Clamp(value, 0, 1);
        }
    }
}