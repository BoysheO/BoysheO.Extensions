using System;
using System.Runtime.CompilerServices;

namespace BoysheO.Extensions
{
    public static class FloatExtension
    {
        /// <summary>
        ///     Returns the smallest integer greater to or equal to f.
        ///     ex. 1.2f=>2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToInt(this float flo)
        {
            return (int) Math.Ceiling(flo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToInt(this float flo)
        {
            return (int) Math.Floor(flo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Abs(this float flo)
        {
            return Math.Abs(flo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(this float value, float min, float max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(this float value)
        {
            return Clamp(value, 0, 1);
        }
    }
}