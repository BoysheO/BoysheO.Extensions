using System;
using System.Runtime.CompilerServices;

namespace BoysheO.Extensions
{
    public static class DoubleExtension
    {
        /// <summary>
        ///     Returns the smallest integer greater to or equal to f.<br />
        ///     ex. 1.2f=>2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToInt(this double flo)
        {
            return (int)Math.Ceiling(flo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToInt(this double flo)
        {
            return (int)Math.Floor(flo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Abs(this double flo)
        {
            return Math.Abs(flo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(this double value, double min, double max)
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
        public static double Max(this double value, double min)
        {
            return Math.Max(value, min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Min(this double value, double min)
        {
            return Math.Min(value, min);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp01(this double value)
        {
            return Clamp(value, 0, 1);
        }
    }
}