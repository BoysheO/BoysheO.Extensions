using System;
using System.Runtime.CompilerServices;

namespace BoysheO.Extensions
{
    public static class DoubleExtension
    {
        /// <summary>
        ///     Returns the smallest integer greater to or equal to f.
        ///     ex. 1.2f=>2
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CeilToInt(this double flo)
        {
            return (int) Math.Ceiling(flo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int FloorToInt(this double flo)
        {
            return (int) Math.Floor(flo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Abs(this double flo)
        {
            return Math.Abs(flo);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(this double value, double min, double max)
        {
            if (value < min)
                value = min;
            else if (value > max)
                value = max;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double ClampMin(this double value, double min)
        {
            return value < min ? min : value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp01(this double value)
        {
            return Clamp(value, 0, 1);
        }
    }
}