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
#if NETSTANDARD2_1_OR_GREATER || NET6_0_OR_GREATER
            return Math.Clamp(value, min, max);
#else
            if (min > max)
            {
                throw new ArgumentException("min cannot be greater than max.");
            }

            if (value < min)
                return min;
            return value > max ? max : value;
#endif
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp01(this float value)
        {
            return Clamp(value, 0, 1);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Max(this float value,float another)
        {
            return Math.Max(value,another);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Min(this float value,float another)
        {
            return Math.Min(value,another);
        }
    }
}