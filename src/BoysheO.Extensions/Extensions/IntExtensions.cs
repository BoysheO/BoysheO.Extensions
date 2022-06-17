using System;
using System.Buffers;
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

        /// <summary>
        /// split int 123=>0,0,0,0,0,0,0,1,2,3
        /// </summary>
        /// <param name="value">source</param>
        /// <param name="buffer">buffer len must be 10</param>
        /// <returns>count of bytes write</returns>
        public static void PositiveIntegerToEachDigit(this int value, Span<int> buffer)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "need >0");
            if (buffer.Length != 10) throw new ArgumentOutOfRangeException(nameof(buffer), "buffer len must be 10");
            var p = buffer.Length - 1;
            while (value > 0)
            {
                if (p < 0) return;
                buffer[p] = value % 10;
                p--;
                value /= 10;
            }

            while (p >= 0)
            {
                buffer[p] = 0;
                p--;
            }
        }
    }
}