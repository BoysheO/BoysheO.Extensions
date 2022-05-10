using System;
using System.Runtime.CompilerServices;

namespace BoysheO.Extensions
{
    public static partial class IntExtensions
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

        /// <summary>
        /// 输出形如+1、-1，+0这类带符号的
        /// </summary>
        /// <param name="value"></param>
        /// <param name="zero">当value为0时，输出这个值，它应为"0"、"+0"、"-0"、" 0"之一</param>
        /// <returns></returns>
        public static string ToStringWithSign(this int value, string zero = "0")
        {
            if (value == 0) return zero;
            return value > 0 ? $"+{value}" : value.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Min(this int value, int another)
        {
            return Math.Min(value, another);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Max(this int value, int another)
        {
            return Math.Max(value, another);
        }
    }
}